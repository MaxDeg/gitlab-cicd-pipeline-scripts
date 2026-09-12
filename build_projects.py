#!/usr/bin/env python3
"""
Build .NET projects.
This script compiles .NET projects for a specified configuration.
"""

import subprocess
import sys
import os
from pathlib import Path
import argparse

def build_dotnet_projects(project_paths=None, configuration='Release', version=None):
    """
    Build .NET projects.
    
    Args:
        project_paths (list): List of paths to .csproj or .sln files.
                            If None, searches for all .sln files.
        configuration (str): Build configuration (Debug/Release). Defaults to Release.
        version (str): Version to apply to the build. Optional.
    
    Returns:
        bool: True if successful, False otherwise
    """
    print(f"Building .NET projects ({configuration})...")
    
    if not project_paths:
        # Find all solution files
        project_paths = list(Path('.').glob('**/*.sln'))
    
    if not project_paths:
        print("Warning: No .NET solution files found")
        return True
    
    for project_path in project_paths:
        print(f"  Building: {project_path}")
        
        cmd = ['dotnet', 'build', str(project_path), '-c', configuration]
        
        # Add version parameter if provided
        if version:
            cmd.extend(['/p:Version=' + version])
        
        try:
            subprocess.run(
                cmd,
                check=True,
                cwd=os.getenv('CI_PROJECT_DIR', '.')
            )
            print(f"  ✓ Successfully built {project_path}")
        except subprocess.CalledProcessError as e:
            print(f"  ✗ Error building {project_path}: {e}", file=sys.stderr)
            return False
    
    return True

def build_npm_projects(npm_paths=None):
    """
    Build JavaScript projects using npm build script.
    
    Args:
        npm_paths (list): List of directories containing package.json files.
                         If None, searches for all package.json files.
    
    Returns:
        bool: True if successful, False otherwise
    """
    print("Building JavaScript projects...")
    
    if not npm_paths:
        # Find all package.json files
        package_files = list(Path('.').glob('**/package.json'))
        npm_paths = [f.parent for f in package_files]
    
    if not npm_paths:
        print("Warning: No JavaScript projects found")
        return True
    
    for npm_path in npm_paths:
        print(f"  Building: {npm_path}")
        try:
            subprocess.run(
                ['npm', 'run', 'build'],
                check=True,
                cwd=str(npm_path)
            )
            print(f"  ✓ Successfully built {npm_path}")
        except subprocess.CalledProcessError as e:
            print(f"  ✗ Error building {npm_path}: {e}", file=sys.stderr)
            return False
    
    return True

def main():
    """Main execution function."""
    parser = argparse.ArgumentParser(description='Build .NET and NPM projects')
    parser.add_argument('--dotnet', nargs='+', help='Paths to .NET projects')
    parser.add_argument('--npm', nargs='+', help='Paths to NPM projects')
    parser.add_argument('--configuration', default='Release', 
                       choices=['Debug', 'Release'],
                       help='Build configuration (default: Release)')
    parser.add_argument('--version', help='Version to apply to builds')
    parser.add_argument('--skip-npm', action='store_true', 
                       help='Skip NPM build')
    parser.add_argument('--skip-dotnet', action='store_true',
                       help='Skip .NET build')
    
    args = parser.parse_args()
    
    print("Starting build process...\n")
    
    dotnet_paths = None
    npm_paths = None
    
    if args.dotnet:
        dotnet_paths = [Path(p) for p in args.dotnet]
    if args.npm:
        npm_paths = [Path(p) for p in args.npm]
    
    results = []
    
    # Build .NET projects
    if not args.skip_dotnet:
        dotnet_success = build_dotnet_projects(dotnet_paths, args.configuration, args.version)
        results.append(('dotnet', dotnet_success))
    
    # Build NPM projects
    if not args.skip_npm:
        npm_success = build_npm_projects(npm_paths)
        results.append(('npm', npm_success))
    
    print("\n" + "="*50)
    all_success = all(success for _, success in results)
    
    for project_type, success in results:
        status = "✓" if success else "✗"
        print(f"{status} {project_type.upper()} build: {'SUCCESS' if success else 'FAILED'}")
    
    if all_success:
        print("="*50)
        print("✓ All projects built successfully!")
        return 0
    else:
        print("="*50)
        print("✗ One or more projects failed to build", file=sys.stderr)
        return 1

if __name__ == '__main__':
    sys.exit(main())
