#!/usr/bin/env python3
"""
Publish .NET projects.
This script builds and publishes .NET applications to various targets.
"""

import subprocess
import sys
import os
from pathlib import Path
import argparse
import shutil

def publish_dotnet_project(project_path, output_dir, configuration='Release', 
                          framework=None, runtime=None, version=None, self_contained=False):
    """
    Publish a single .NET project.
    
    Args:
        project_path (Path): Path to the .csproj file
        output_dir (Path): Output directory for published files
        configuration (str): Build configuration (Debug/Release)
        framework (str): Target framework (e.g., net6.0, net7.0)
        runtime (str): Runtime identifier (e.g., win-x64, linux-x64)
        version (str): Version to apply
        self_contained (bool): Publish as self-contained deployment
    
    Returns:
        bool: True if successful, False otherwise
    """
    print(f"  Publishing: {project_path}")
    
    # Create output directory for this project
    project_output = output_dir / project_path.stem
    project_output.mkdir(parents=True, exist_ok=True)
    
    cmd = [
        'dotnet', 'publish',
        str(project_path),
        '-c', configuration,
        '-o', str(project_output)
    ]
    
    # Add optional parameters
    if framework:
        cmd.extend(['-f', framework])
    
    if runtime:
        cmd.extend(['-r', runtime])
    
    if version:
        cmd.append(f'/p:Version={version}')
    
    if self_contained:
        cmd.append('--self-contained')
    
    try:
        subprocess.run(
            cmd,
            check=True,
            cwd=os.getenv('CI_PROJECT_DIR', '.')
        )
        print(f"    ✓ Published to {project_output}")
        return True
    except subprocess.CalledProcessError as e:
        print(f"    ✗ Error publishing {project_path}: {e}", file=sys.stderr)
        return False

def publish_dotnet_projects(project_paths=None, output_dir='./publish', 
                           configuration='Release', framework=None, 
                           runtime=None, version=None, self_contained=False):
    """
    Publish multiple .NET projects.
    
    Args:
        project_paths (list): List of paths to .csproj files.
                            If None, searches for publishable projects.
        output_dir (str): Base output directory for published files
        configuration (str): Build configuration
        framework (str): Target framework
        runtime (str): Runtime identifier
        version (str): Version to apply
        self_contained (bool): Publish as self-contained
    
    Returns:
        bool: True if all projects published successfully, False otherwise
    """
    print(f"Publishing .NET projects ({configuration})...\n")
    
    output_path = Path(output_dir)
    
    if not project_paths:
        # Find all publishable projects (excluding test projects)
        all_projects = list(Path('.').glob('**/*.csproj'))
        project_paths = [p for p in all_projects 
                        if 'test' not in p.name.lower()]
    
    if not project_paths:
        print("Warning: No .NET projects found to publish")
        return True
    
    all_published = True
    published_projects = []
    
    for project_path in project_paths:
        success = publish_dotnet_project(
            project_path,
            output_path,
            configuration,
            framework,
            runtime,
            version,
            self_contained
        )
        all_published = all_published and success
        
        if success:
            published_projects.append(str(project_path.stem))
    
    return all_published, published_projects

def create_artifacts_summary(output_dir, published_projects):
    """
    Create a summary file of published artifacts.
    
    Args:
        output_dir (Path): Output directory containing published files
        published_projects (list): List of published project names
    """
    summary_file = Path(output_dir) / 'PUBLISH_SUMMARY.txt'
    
    with open(summary_file, 'w') as f:
        f.write("Published .NET Projects Summary\n")
        f.write("=" * 50 + "\n\n")
        
        for project in published_projects:
            project_dir = Path(output_dir) / project
            if project_dir.exists():
                file_count = len(list(project_dir.rglob('*')))
                size = sum(f.stat().st_size for f in project_dir.rglob('*') if f.is_file())
                size_mb = size / (1024 * 1024)
                
                f.write(f"Project: {project}\n")
                f.write(f"  Location: {project_dir}\n")
                f.write(f"  Files: {file_count}\n")
                f.write(f"  Size: {size_mb:.2f} MB\n\n")
    
    print(f"\n✓ Summary written to {summary_file}")

def create_docker_artifacts(output_dir, project_name, port=None):
    """
    Create optional Docker artifact files for published projects.
    
    Args:
        output_dir (Path): Output directory
        project_name (str): Project name
        port (int): Port for web applications (optional)
    """
    dockerfile = Path(output_dir) / project_name / 'Dockerfile'
    project_output = Path(output_dir) / project_name
    
    dockerfile_content = f"""FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "{project_name}.dll"]
"""
    
    if port:
        dockerfile_content += f"EXPOSE {port}\n"
    
    with open(dockerfile, 'w') as f:
        f.write(dockerfile_content)
    
    print(f"  Generated Dockerfile for {project_name}")

def main():
    """Main execution function."""
    parser = argparse.ArgumentParser(description='Publish .NET projects')
    parser.add_argument('--projects', nargs='+', help='Paths to .csproj files to publish')
    parser.add_argument('--output', default='./publish',
                       help='Output directory for published files (default: ./publish)')
    parser.add_argument('--configuration', default='Release',
                       choices=['Debug', 'Release'],
                       help='Build configuration (default: Release)')
    parser.add_argument('--framework', help='Target framework (e.g., net6.0, net7.0)')
    parser.add_argument('--runtime', help='Runtime identifier (e.g., win-x64, linux-x64)')
    parser.add_argument('--version', help='Version to apply to published files')
    parser.add_argument('--self-contained', action='store_true',
                       help='Publish as self-contained deployment')
    parser.add_argument('--summary', action='store_true',
                       help='Generate publish summary report')
    parser.add_argument('--docker', action='store_true',
                       help='Generate Dockerfile for each project')
    
    args = parser.parse_args()
    
    print("Starting .NET publish process...\n")
    
    project_paths = None
    if args.projects:
        project_paths = [Path(p) for p in args.projects]
    
    # Publish projects
    all_published, published_projects = publish_dotnet_projects(
        project_paths,
        args.output,
        args.configuration,
        args.framework,
        args.runtime,
        args.version,
        args.self_contained
    )
    
    if all_published and published_projects:
        print("\n" + "="*50)
        print(f"✓ Successfully published {len(published_projects)} project(s)")
        print(f"  Output directory: {args.output}")
        print("="*50 + "\n")
        
        # Generate optional artifacts
        if args.summary:
            create_artifacts_summary(args.output, published_projects)
        
        if args.docker:
            print("\nGenerating Docker artifacts...")
            for project in published_projects:
                create_docker_artifacts(Path(args.output), project)
        
        return 0
    else:
        print("\n✗ One or more projects failed to publish", file=sys.stderr)
        return 1

if __name__ == '__main__':
    sys.exit(main())
