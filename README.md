# NintenTools.Bfres

This .NET library provides easy access to data stored in the BFRES Nintendo graphics archive file format (most prominently used to store 3D game models), allows to modify the data and save new files.

More details are found on the [wiki](https://github.com/Syroot/NintenTools.Bfres/wiki).

## Supported Features

- Loading all subfiles and their sections of a BFRES file (at least version 3.x or newer). 
- Save new or modified BFRES files written from scratch.
- Parsing BFRES files visually in [010 Editor](https://www.sweetscape.com/010editor/) with the provided [binary templates](https://github.com/Syroot/NintenTools.Bfres/tree/master/other/010_editor).

The following features are **not yet implemented**, but planned:
- Methods providing easy access to vertex data from `VertexBuffer` instances in combination with `VertexAttrib`.
- Simplified modification of `AnimCurve` instances and better handling of key data.
- Classes mapping typical `ExternalFile` contents (like BFSHA shader data), manually loadable on demand.

The following features are **not planned**:
- Accessing raw header data (like file offsets). Since this can be useful for injection tools, it might be implemented on demand (please submit a feature request).
- Deswizzling texture data.

## NuGet Package

It is not required to download the library in source and compile it yourself, as a typically up-to-date [NuGet package](https://www.nuget.org/packages/Syroot.NintenTools.Bfres) exists.
