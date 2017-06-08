# NintenTools.Bfres

The goal of this .NET library is to provide easy access to data stored in the BFRES Nintendo graphics archive file format (most prominently used to store 3D game models).

## Supported Features

- The library is in an alpha state - your code might break when upgrading to newer alpha versions.
- Loading all subfiles and their sections of a BFRES file (at least version 3.x or newer):

    | Signature | Description                | Class             |
    |:---------:|----------------------------|-------------------|
    | FRES      | Main File                  | `ResFile`         |
    | FMDL      | Model                      | `Model`           |
    | FTEX      | Texture                    | `Texture`         |
    | FSKA      | Skeletal Animation         | `SkeletalAnim`    |
    | FSHU      | Shader Parameter Animation | `ShaderParamAnim` |
    | FTXP      | Texture Pattern Animation  | `TexPatternAnim`  |
    | FVIS      | Visibility Animation       | `VisibilityAnim`  |
    | FSHA      | Shape Animation            | `ShapeAnim`       |
    | FSCN      | Scene Animation            | `SceneAnim`       |
    | -         | External File              | `ExternalFile`    |
 
- Parsing BFRES files visually in [010 Editor](https://www.sweetscape.com/010editor/) with the provided [binary templates](https://github.com/Syroot/NintenTools.Bfres/tree/master/other/010%20Binary%20Templates).

The following features are **not yet implemented**, but planned:
- Methods simplifying access to vertex data from `VertexBuffer` instances in combination with `VertexAttrib`.
- Classes mapping typical `ExternalFile` contents (like BFSHAR shader data), manually loadable on demand.
- Storing (modified) data into new files.

The following features are **not planned**:
- Accessing raw header data (like file offsets). While the library wraps headers, they are not exposed and dismissed after loading the referenced data into classes. Since this might be useful for injection tools, it might be implemented on demand (please submit a feature request).
- Deswizzling texture data.

## NuGet Package

It is not required to download the library in source and compile it yourself, as a typically up-to-date [NuGet package](https://www.nuget.org/packages/Syroot.NintenTools.Bfres) exists.
