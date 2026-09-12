#!/usr/bin/env python3
"""
Run tests for .NET and JavaScript projects.
This script executes unit tests and generates coverage reports.
"""

import subprocess
import sys
import os
from pathlib import Path
import argparse
import json

def run_dotnet_tests(project_paths=None, configuration='Release', collect_coverage=False):
    """
    Run .NET unit tests.
    
    Args:
        project_paths (list): List of paths to test projects.
                            If None, searches for all *Tests.csproj files.
        configuration (str): Build configuration (Debug/Release). Defaults to Release.
        collect_coverage (bool): Whether to collect code coverage. Defaults to False.
    
    Returns:
        bool: True if all tests passed, False otherwise
    """
    print(f"Running .NET tests ({configuration})...")
    
    if not project_paths:
        # Find all test projects
        project_paths = list(Path('.').glob('**/*Tests.csproj'))
        project_paths.extend(Path('.').glob('**/*.Tests.csproj'))
    
    if not project_paths:
        print("Warning: No .NET test projects found")
        return True
    
    all_passed = True
    
    for project_path in project_paths:
        print(f"  Testing: {project_path}")
        
        cmd = ['dotnet', 'test', str(project_path), '-c', configuration]
        
        # Add code coverage if requested
        if collect_coverage:
            cmd.extend([
                '/p:CollectCoverage=true',
                '/p:CoverletOutputFormat=opencover',
                '/p:CoverletOutput=./coverage/'
            ])
        
        # Add logging options
        cmd.extend(['--logger', 'console;verbosity=minimal'])
        
        try:
            subprocess.run(
                cmd,
                check=True,
                cwd=os.getenv('CI_PROJECT_DIR', '.')
            )
            print(f"  ✓ Tests passed for {project_path}")
        except subprocess.CalledProcessError as e:
            print(f"  ✗ Tests failed for {project_path}: {e}", file=sys.stderr)
            all_passed = False
    
    return all_passed

def run_npm_tests(npm_paths=None, coverage=False):
    """
    Run JavaScript unit tests using npm test.
    
    Args:
        npm_paths (list): List of directories containing package.json files.
                         If None, searches for all package.json files.
        coverage (bool): Whether to collect code coverage. Defaults to False.
    
    Returns:
        bool: True if all tests passed, False otherwise
    """
    print("Running JavaScript tests...")
    
    if not npm_paths:
        # Find all package.json files
        package_files = list(Path('.').glob('**/package.json'))
        npm_paths = [f.parent for f in package_files]
    
    if not npm_paths:
        print("Warning: No JavaScript projects found")
        return True
    
    all_passed = True
    
    for npm_path in npm_paths:
        print(f"  Testing: {npm_path}")
        
        # Check if package.json has test script
        package_json_path = npm_path / 'package.json'
        if not package_json_path.exists():
            print(f"    Skipping: No package.json found")
            continue
        
        try:
            with open(package_json_path, 'r') as f:
                package_data = json.load(f)
                scripts = package_data.get('scripts', {})
                
                if 'test' not in scripts:
                    print(f"    Skipping: No test script defined")
                    continue
        except (json.JSONDecodeError, IOError) as e:
            print(f"    ✗ Error reading package.json: {e}", file=sys.stderr)
            all_passed = False
            continue
        
        # Run tests
        cmd = ['npm', 'test']
        
        if coverage:
            cmd.append('--')
            cmd.append('--coverage')
        
        try:
            subprocess.run(
                cmd,
                check=True,
                cwd=str(npm_path)
            )
            print(f"  ✓ Tests passed for {npm_path}")
        except subprocess.CalledProcessError as e:
            print(f"  ✗ Tests failed for {npm_path}: {e}", file=sys.stderr)
            all_passed = False
    
    return all_passed

def main():
    """Main execution function."""
    parser = argparse.ArgumentParser(description='Run .NET and NPM tests')
    parser.add_argument('--dotnet', nargs='+', help='Paths to .NET test projects')
    parser.add_argument('--npm', nargs='+', help='Paths to NPM projects')
    parser.add_argument('--configuration', default='Release',
                       choices=['Debug', 'Release'],
                       help='Build configuration for .NET tests (default: Release)')
    parser.add_argument('--coverage', action='store_true',
                       help='Collect code coverage for both .NET and NPM')
    parser.add_argument('--skip-npm', action='store_true',
                       help='Skip NPM tests')
    parser.add_argument('--skip-dotnet', action='store_true',
                       help='Skip .NET tests')
    
    args = parser.parse_args()
    
    print("Starting test execution...\n")
    
    dotnet_paths = None
    npm_paths = None
    
    if args.dotnet:
        dotnet_paths = [Path(p) for p in args.dotnet]
    if args.npm:
        npm_paths = [Path(p) for p in args.npm]
    
    results = []
    
    # Run .NET tests
    if not args.skip_dotnet:
        dotnet_passed = run_dotnet_tests(dotnet_paths, args.configuration, args.coverage)
        results.append(('dotnet', dotnet_passed))
    
    # Run NPM tests
    if not args.skip_npm:
        npm_passed = run_npm_tests(npm_paths, args.coverage)
        results.append(('npm', npm_passed))
    
    print("\n" + "="*50)
    all_passed = all(passed for _, passed in results)
    
    for project_type, passed in results:
        status = "✓" if passed else "✗"
        print(f"{status} {project_type.upper()} tests: {'PASSED' if passed else 'FAILED'}")
    
    if all_passed:
        print("="*50)
        print("✓ All tests passed successfully!")
        if args.coverage:
            print("  Coverage reports generated")
        return 0
    else:
        print("="*50)
        print("✗ One or more test suites failed", file=sys.stderr)
        return 1

if __name__ == '__main__':
    sys.exit(main())
