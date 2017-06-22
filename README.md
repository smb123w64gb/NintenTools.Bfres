# NintenTools.Bfres

The goal of this .NET library is to provide easy access to data stored in the BFRES Nintendo graphics archive file format (most prominently used to store 3D game models) and store data in new files.

The library has now gone out of beta.
- It has been tested rewriting Mario Kart 8 object, character and track models (BFRES version 3.4.0.4), which means compatibility with other games might require some work to be optimal.
- Usability of the library will improve over time with new helper methods and classes. If you have ideas, feel free to send feature requests in the [issues section](https://github.com/Syroot/NintenTools.Bfres/issues).

## Supported Features

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
 
- Saving existing BFRES files or original ones from scratch. This is experimental and unfinished.
- Parsing BFRES files visually in [010 Editor](https://www.sweetscape.com/010editor/) with the provided [binary templates](https://github.com/Syroot/NintenTools.Bfres/tree/master/other/010_editor).

The following features are **not yet implemented**, but planned:
- Methods simplifying access to vertex data from `VertexBuffer` instances in combination with `VertexAttrib`.
- Classes mapping typical `ExternalFile` contents (like BFSHA shader data), manually loadable on demand.

The following features are **not planned**:
- Accessing raw header data (like file offsets). While the library wraps headers, they are not exposed and dismissed after loading the referenced data into classes. Since this might be useful for injection tools, it might be implemented on demand (please submit a feature request).
- Deswizzling texture data.

## NuGet Package

It is not required to download the library in source and compile it yourself, as a typically up-to-date [NuGet package](https://www.nuget.org/packages/Syroot.NintenTools.Bfres) exists.
