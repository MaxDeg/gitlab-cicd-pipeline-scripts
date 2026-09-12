# GitLab CI/CD Pipeline Scripts

This repository contains Python scripts designed to automate .NET and JavaScript project builds in GitLab CI/CD pipelines. These scripts handle version calculation, dependency restoration, building, testing, and publishing of applications.

## Overview

The pipeline supports a complete CI/CD workflow for applications with both .NET backend and JavaScript frontend components:

```
Version Calculation → Restore Dependencies → Build → Test → Publish
```

## Scripts

### 1. `calculate_version.py`
Calculates application version using GitVersion.

**Features:**
- Executes GitVersion to generate semantic versioning
- Exports version information as environment variables
- Supports Major.Minor.Patch versioning
- Generates `version.env` file for downstream stages

**Usage:**
```bash
python3 calculate_version.py
```

**Output:**
- `version.env` file with VERSION, MAJOR, MINOR, PATCH, BUILD_METADATA, COMMIT_SHA
- GitLab CI/CD environment variables

**Prerequisites:**
- GitVersion installed globally: `dotnet tool install --global GitVersion.Tool`

---

### 2. `restore_projects.py`
Restores dependencies for .NET and JavaScript projects.

**Features:**
- Automatically discovers .NET solutions (.sln) and npm projects (package.json)
- Runs `dotnet restore` for .NET projects
- Runs `npm ci` for JavaScript projects
- Supports specific project paths
- Provides detailed logging

**Usage:**
```bash
# Auto-discover all projects
python3 restore_projects.py

# Specify specific projects
python3 restore_projects.py --dotnet src/MyProject.sln --npm frontend backend/app
```

**Supported Options:**
- `--dotnet` - Paths to .NET solution/project files
- `--npm` - Paths to npm project directories

---

### 3. `build_projects.py`
Builds .NET and JavaScript projects.

**Features:**
- Builds .NET projects with specified configuration (Debug/Release)
- Executes npm build scripts for JavaScript projects
- Supports version injection into assemblies
- Can skip specific project types
- Generates build artifacts

**Usage:**
```bash
# Build all projects with Release configuration
python3 build_projects.py --configuration Release

# Build with version
python3 build_projects.py --configuration Release --version 1.2.3

# Skip npm build
python3 build_projects.py --skip-npm

# Specify specific projects
python3 build_projects.py --dotnet src/API.csproj src/Service.csproj
```

**Supported Options:**
- `--configuration` - Debug or Release (default: Release)
- `--version` - Version to inject into builds
- `--dotnet` - Specific .NET projects
- `--npm` - Specific npm projects
- `--skip-npm` - Skip JavaScript builds
- `--skip-dotnet` - Skip .NET builds

---

### 4. `run_tests.py`
Executes unit tests for .NET and JavaScript projects.

**Features:**
- Discovers and runs .NET unit tests (.Tests.csproj files)
- Executes JavaScript tests via npm test
- Supports code coverage collection
- Generates JUnit XML reports
- Provides detailed test results

**Usage:**
```bash
# Run all tests
python3 run_tests.py

# Run tests with coverage
python3 run_tests.py --coverage

# Skip JavaScript tests
python3 run_tests.py --skip-npm

# Specify test projects
python3 run_tests.py --dotnet src/MyProject.Tests.csproj
```

**Supported Options:**
- `--configuration` - Debug or Release (default: Release)
- `--coverage` - Collect code coverage reports
- `--dotnet` - Specific .NET test projects
- `--npm` - Specific npm projects
- `--skip-npm` - Skip JavaScript tests
- `--skip-dotnet` - Skip .NET tests

**Output:**
- TestResults in `**/TestResults/` directories
- Coverage reports in `**/coverage/` directories
- JUnit XML format for CI/CD integration

---

### 5. `publish_dotnet.py`
Publishes .NET applications for distribution.

**Features:**
- Publishes .NET projects to output directories
- Supports framework targeting (net6.0, net7.0, etc.)
- Supports runtime selection (win-x64, linux-x64, etc.)
- Generates deployment summaries
- Optional Dockerfile generation
- Self-contained deployment support

**Usage:**
```bash
# Publish all projects
python3 publish_dotnet.py

# Publish with specific output directory
python3 publish_dotnet.py --output ./artifacts

# Publish for Linux
python3 publish_dotnet.py --runtime linux-x64

# Generate summary and Dockerfiles
python3 publish_dotnet.py --summary --docker

# Publish specific projects with version
python3 publish_dotnet.py --projects src/API.csproj src/Worker.csproj --version 1.2.3
```

**Supported Options:**
- `--output` - Output directory (default: ./publish)
- `--configuration` - Debug or Release (default: Release)
- `--framework` - Target framework (e.g., net6.0, net7.0)
- `--runtime` - Runtime identifier (e.g., win-x64, linux-x64)
- `--version` - Version to apply
- `--projects` - Specific projects to publish
- `--self-contained` - Publish as self-contained deployment
- `--summary` - Generate publish summary report
- `--docker` - Generate Dockerfile for each project

**Output:**
- Published artifacts in specified output directory
- PUBLISH_SUMMARY.txt with file counts and sizes
- Optional Dockerfile for containerization

---

## GitLab CI/CD Pipeline

The `.gitlab-ci.yml` file defines the complete pipeline with the following stages:

### Pipeline Stages

1. **Version** (`calculate_version`)
   - Calculates semantic version using GitVersion
   - Exports version for downstream stages

2. **Restore** (`restore_dependencies`)
   - Installs .NET and npm dependencies
   - Caches dependencies for faster builds

3. **Build** (`build_projects`)
   - Compiles .NET assemblies
   - Builds JavaScript applications

4. **Test** (`run_tests`)
   - Runs .NET unit tests
   - Runs JavaScript unit tests
   - Collects code coverage

5. **Publish** (`publish_artifacts`)
   - Publishes .NET applications
   - Generates deployment packages
   - Manual trigger (requires approval)

### Environment Variables

```yaml
DOTNET_VERSION: "7.0"
NODE_VERSION: "18"
```

### Cache Configuration

The pipeline caches:
- `.nuget/` - NuGet packages
- `node_modules/` - npm dependencies

### Artifact Artifacts

Each stage generates artifacts for downstream consumption:
- Version artifacts: `version.env`
- Build artifacts: `bin/`, `dist/`
- Test artifacts: `TestResults/`, `coverage/`
- Publish artifacts: `publish/`

---

## Installation

### Prerequisites

- Docker (for CI/CD execution)
- .NET SDK 7.0+ (local development)
- Node.js 16+ (local development)
- Python 3.8+ (for scripts)
- GitVersion Tool: `dotnet tool install --global GitVersion.Tool`

### Setup

1. Clone the repository:
```bash
git clone <repository-url>
cd gitlab-cicd-pipeline-scripts
```

2. Make scripts executable:
```bash
chmod +x *.py
```

3. Copy scripts to your project:
```bash
cp *.py /path/to/your/project/
cp .gitlab-ci.yml /path/to/your/project/
```

4. Update `.gitlab-ci.yml` with your project-specific paths

---

## Local Testing

Test scripts locally before committing to CI/CD:

```bash
# Test version calculation
python3 calculate_version.py

# Test restoration
python3 restore_projects.py

# Test build
python3 build_projects.py --configuration Debug

# Test with specific projects
python3 run_tests.py --dotnet src/MyProject.Tests.csproj

# Test publish
python3 publish_dotnet.py --output ./test-publish
```

---

## Advanced Usage

### Custom Project Discovery

To customize which projects are discovered, modify the glob patterns in each script:

**For .NET projects:**
```python
# Find all solution files
project_paths = list(Path('.').glob('**/*.sln'))
```

**For npm projects:**
```python
# Find all package.json files
package_files = list(Path('.').glob('**/package.json'))
```

### Environment Variable Integration

Scripts export results as environment variables for GitLab CI/CD:

```bash
# In calculate_version.py
print(f"::set-env name=VERSION::{version_data.get('FullSemVer', 'unknown')}")
```

### Error Handling

All scripts include comprehensive error handling:
- Non-zero exit codes on failure
- Detailed error messages to stderr
- Graceful degradation (warnings, not errors)

---

## Troubleshooting

### Build Failures

**Issue:** "dotnet restore failed"
- **Solution:** Ensure .csproj files are valid and NuGet sources are accessible

**Issue:** "npm ci failed"
- **Solution:** Check package.json validity and npm registry connectivity

### Version Calculation Issues

**Issue:** "GitVersion not found"
- **Solution:** Install GitVersion: `dotnet tool install --global GitVersion.Tool`

**Issue:** "Version calculation failed"
- **Solution:** Ensure repository has valid Git history with tags

### Test Failures

**Issue:** "No test projects found"
- **Solution:** Name test projects with `.Tests` suffix (e.g., `MyProject.Tests.csproj`)

**Issue:** "npm test failed"
- **Solution:** Ensure `package.json` has a `test` script defined

---

## Contributing

To add new functionality:

1. Create a new script following the existing pattern
2. Include comprehensive docstrings
3. Add error handling
4. Update this README
5. Test locally before committing

---

## License

MIT License - feel free to use and modify as needed.

---

## Support

For issues or questions:
1. Check the Troubleshooting section
2. Review script docstrings
3. Run scripts with `--help` for options
4. Check GitLab CI/CD logs for detailed error messages
