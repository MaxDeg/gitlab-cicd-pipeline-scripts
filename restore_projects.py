#!/usr/bin/env python3
"""
Restore .NET and JavaScript projects.
This script restores dependencies for both .NET and npm projects.
"""

import subprocess
import sys
import os
from pathlib import Path

def restore_dotnet_projects(project_paths=None):
    """
    Restore .NET project dependencies.
    
    Args:
        project_paths (list): List of paths to .csproj or .sln files.
                            If None, searches for all .sln files.
    
    Returns:
        bool: True if successful, False otherwise
    """
    print("Restoring .NET projects...")
    
    if not project_paths:
        # Find all solution files
        project_paths = list(Path('.').glob('**/*.sln'))
    
    if not project_paths:
        print("Warning: No .NET solution files found")
        return True
    
    for project_path in project_paths:
        print(f"  Restoring: {project_path}")
        try:
            subprocess.run(
                ['dotnet', 'restore', str(project_path)],
                check=True,
                cwd=os.getenv('CI_PROJECT_DIR', '.')
            )
            print(f"  ✓ Successfully restored {project_path}")
        except subprocess.CalledProcessError as e:
            print(f"  ✗ Error restoring {project_path}: {e}", file=sys.stderr)
            return False
    
    return True

def restore_npm_projects(npm_paths=None):
    """
    Restore JavaScript project dependencies using npm.
    
    Args:
        npm_paths (list): List of directories containing package.json files.
                         If None, searches for all package.json files.
    
    Returns:
        bool: True if successful, False otherwise
    """
    print("Restoring JavaScript projects...")
    
    if not npm_paths:
        # Find all package.json files
        package_files = list(Path('.').glob('**/package.json'))
        npm_paths = [f.parent for f in package_files]
    
    if not npm_paths:
        print("Warning: No JavaScript projects found")
        return True
    
    for npm_path in npm_paths:
        print(f"  Restoring: {npm_path}")
        try:
            subprocess.run(
                ['npm', 'ci'],
                check=True,
                cwd=str(npm_path)
            )
            print(f"  ✓ Successfully restored {npm_path}")
        except subprocess.CalledProcessError as e:
            print(f"  ✗ Error restoring {npm_path}: {e}", file=sys.stderr)
            return False
    
    return True

def main():
    """Main execution function."""
    print("Starting project restoration process...\n")
    
    # You can pass specific paths as command-line arguments
    dotnet_paths = None
    npm_paths = None
    
    if len(sys.argv) > 1:
        # Parse arguments for specific paths
        # Example: restore_projects.py --dotnet "src/MyProject.sln" --npm "frontend"
        import argparse
        parser = argparse.ArgumentParser(description='Restore .NET and NPM projects')
        parser.add_argument('--dotnet', nargs='+', help='Paths to .NET projects')
        parser.add_argument('--npm', nargs='+', help='Paths to NPM projects')
        args = parser.parse_args()
        
        if args.dotnet:
            dotnet_paths = [Path(p) for p in args.dotnet]
        if args.npm:
            npm_paths = [Path(p) for p in args.npm]
    
    # Restore projects
    dotnet_success = restore_dotnet_projects(dotnet_paths)
    npm_success = restore_npm_projects(npm_paths)
    
    if dotnet_success and npm_success:
        print("\n✓ All projects restored successfully!")
        return 0
    else:
        print("\n✗ One or more projects failed to restore", file=sys.stderr)
        return 1

if __name__ == '__main__':
    sys.exit(main())
