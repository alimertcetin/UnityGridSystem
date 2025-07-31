# PACKAGE-NAME

## Npm Publish Checklist

1. [ ] Set NPM_TOKEN
    1. Go to settings
    2. Select Actions under Secrets and Variables
    3. New repository secret
    4. Set name as "NPM_TOKEN" and set the Secret that you get from npm
2. [ ] Clone the repo to directly inside unity project
    - This will create .meta files
3. [ ] Create "Prepare-Npm" branch
4. [ ] Run "npm init" in the root directory
5. [ ] Edit packages.json with relative information about the package
    - Do not update the version
6. [ ] Update docfx_project/api/index.md file
