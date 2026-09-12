#!/usr/bin/env python3
"""
Calculate application version using GitVersion.
This script runs GitVersion and exports the version information.
"""

import subprocess
import json
import sys
import os

def run_gitversion():
    """
    Execute GitVersion and retrieve version information.
    
    Returns:
        dict: GitVersion output containing version information
    """
    try:
        result = subprocess.run(
            ['dotnet', 'tool', 'run', 'gitversion'],
            capture_output=True,
            text=True,
            check=True
        )
        
        version_data = json.loads(result.stdout)
        return version_data
    
    except subprocess.CalledProcessError as e:
        print(f"Error running GitVersion: {e.stderr}", file=sys.stderr)
        sys.exit(1)
    except json.JSONDecodeError as e:
        print(f"Error parsing GitVersion output: {e}", file=sys.stderr)
        sys.exit(1)

def export_version_variables(version_data):
    """
    Export version variables as GitLab CI/CD variables.
    
    Args:
        version_data (dict): Version information from GitVersion
    """
    env_file = os.getenv('CI_PROJECT_DIR', '.') + '/version.env'
    
    with open(env_file, 'w') as f:
        f.write(f"VERSION={version_data.get('FullSemVer', 'unknown')}\n")
        f.write(f"MAJOR={version_data.get('Major', '0')}\n")
        f.write(f"MINOR={version_data.get('Minor', '0')}\n")
        f.write(f"PATCH={version_data.get('Patch', '0')}\n")
        f.write(f"BUILD_METADATA={version_data.get('BuildMetadata', '')}\n")
        f.write(f"COMMIT_SHA={version_data.get('Sha', 'unknown')}\n")
    
    print(f"Version information exported to {env_file}")
    print(f"Full Version: {version_data.get('FullSemVer', 'unknown')}")
    
    # Also print for immediate use
    print(f"::set-env name=VERSION::{version_data.get('FullSemVer', 'unknown')}")

def main():
    """Main execution function."""
    print("Calculating application version using GitVersion...")
    
    version_data = run_gitversion()
    export_version_variables(version_data)
    
    print("Version calculation completed successfully!")
    return 0

if __name__ == '__main__':
    sys.exit(main())
