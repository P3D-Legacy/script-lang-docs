# script-lang-docs
Contains the github page for the script V3 language docs.

Docs page: https://p3d-legacy.github.io/script-lang-docs/

## Building docs
To build the docs/ directory, which is used by the site, clone the repo and open the DocsEmitter.sln file in Visual Studio.

Navigate to the DocsEmitter/Program.cs file and adjust the relative path to the Pokemon 3D repo in the `P3D_GAME_DIR` variable.

The default assumes that both repos have been cloned into the same root folder and that the Pokemon 3D repo folder is called "p3d".

Default assumed folder structure:

    |- p3d
      |- 2.5DHero
        |- 2.5DHero
         ...
    |- p3d-script-lang-docs
      |- DocsEmitter.sln

Afterwards, you might need to adjust the `ASSEMBLY_PATH` variable as well, if you are not targeting Debug\DesktopGL.

Then, build the solution (or hit Run/F5) and the output will be generated into the docs/ folder.
