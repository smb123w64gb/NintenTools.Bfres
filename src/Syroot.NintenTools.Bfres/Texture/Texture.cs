using System;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ResFile"/>, storing multi-dimensional texture data.
    /// </summary>
    [DebuggerDisplay(nameof(Texture) + " {" + nameof(Name) + "}")]
    public class Texture : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the shape of the texture.
        /// </summary>
        public GX2SurfaceDim Dim { get; set; }

        /// <summary>
        /// Gets or sets the width of the texture.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the texture.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Gets or sets the depth of the texture.
        /// </summary>
        public uint Depth { get; set; }

        /// <summary>
        /// Gets or sets the number of mipmaps stored in the <see cref="MipData"/>.
        /// </summary>
        public uint MipCount { get; set; }

        /// <summary>
        /// Gets or sets the desired texture data buffer format.
        /// </summary>
        public GX2SurfaceFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the number of samples for the texture.
        /// </summary>
        public GX2AAMode AAMode { get; set; }

        /// <summary>
        /// Gets or sets the texture data usage hint.
        /// </summary>
        public GX2SurfaceUse Use { get; set; }

        /// <summary>
        /// Gets or sets the tiling mode.
        /// </summary>
        public GX2TileMode TileMode { get; set; }

        /// <summary>
        /// Gets or sets the swizzling value.
        /// </summary>
        public uint Swizzle { get; set; }

        /// <summary>
        /// Gets or sets the swizzling alignment.
        /// </summary>
        public uint Alignment { get; set; }

        /// <summary>
        /// Gets or sets the pixel swizzling stride.
        /// </summary>
        public uint Pitch { get; set; }

        /// <summary>
        /// Gets or sets the offsets in the <see cref="MipData"/> array to the data of the mipmap level corresponding
        /// to the array index.
        /// </summary>
        public uint[] MipOffsets { get; set; }

        public uint ViewMipFirst { get; set; }

        public uint ViewMipCount { get; set; }

        public uint ViewSliceFirst { get; set; }

        public uint ViewSliceCount { get; set; }

        /// <summary>
        /// Gets or sets the GX2CompSel value.
        /// </summary>
        public uint CompSel { get; set; }
        
        public uint[] Regs { get; set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{Texture}"/> instances.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_name != value)
                {
                    _name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the raw texture data bytes.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the raw mipmap level data bytes for all levels.
        /// </summary>
        public byte[] MipData { get; set; }

        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            TextureHead head = new TextureHead(loader);
            Dim = head.Dim;
            Width = head.Width;
            Height = head.Height;
            Depth = head.Depth;
            MipCount = head.NumMips;
            Format = head.Format;
            AAMode = head.AAMode;
            Use = head.Use;
            TileMode = head.TileMode;
            Swizzle = head.Swizzle;
            Alignment = head.Alignment;
            Pitch = head.Pitch;
            MipOffsets = head.MipOffsets;
            ViewMipFirst = head.ViewFirstMip;
            ViewMipCount = head.ViewNumMips;
            ViewSliceFirst = head.ViewFirstSlice;
            ViewSliceCount = head.ViewNumSlices;
            CompSel = head.CompSel;
            Regs = head.Regs;
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);

            loader.Position = head.OfsData;
            Data = loader.ReadBytes((int)head.SizData);

            loader.Position = head.OfsMipData;
            MipData = loader.ReadBytes((int)head.SizMipData);

            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="Texture"/> instance.
    /// </summary>
    internal class TextureHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FTEX";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal GX2SurfaceDim Dim;
        internal uint Width;
        internal uint Height;
        internal uint Depth;
        internal uint NumMips;
        internal GX2SurfaceFormat Format;
        internal GX2AAMode AAMode;
        internal GX2SurfaceUse Use;
        internal uint SizData;
        internal uint ImagePointer;
        internal uint SizMipData;
        internal uint MipPointer;
        internal GX2TileMode TileMode;
        internal uint Swizzle;
        internal uint Alignment;
        internal uint Pitch;
        internal uint[] MipOffsets;
        internal uint ViewFirstMip;
        internal uint ViewNumMips;
        internal uint ViewFirstSlice;
        internal uint ViewNumSlices;
        internal uint CompSel; // TODO: Map GX2CompSel.
        internal uint[] Regs;
        internal uint Handle;
        internal uint ArrayLength;
        internal uint OfsName;
        internal uint OfsPath;
        internal uint OfsData;
        internal uint OfsMipData;
        internal uint OfsUserDataDict;
        internal ushort NumUserData;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal TextureHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            Dim = loader.ReadEnum<GX2SurfaceDim>(true);
            Width = loader.ReadUInt32();
            Height = loader.ReadUInt32();
            Depth = loader.ReadUInt32();
            NumMips = loader.ReadUInt32();
            Format = loader.ReadEnum<GX2SurfaceFormat>(true);
            AAMode = loader.ReadEnum<GX2AAMode>(true);
            Use = loader.ReadEnum<GX2SurfaceUse>(true);
            SizData = loader.ReadUInt32();
            ImagePointer = loader.ReadUInt32();
            SizMipData = loader.ReadUInt32();
            MipPointer = loader.ReadUInt32();
            TileMode = loader.ReadEnum<GX2TileMode>(true);
            Swizzle = loader.ReadUInt32();
            Alignment = loader.ReadUInt32();
            Pitch = loader.ReadUInt32();
            MipOffsets = loader.ReadUInt32s(13);
            ViewFirstMip = loader.ReadUInt32();
            ViewNumMips = loader.ReadUInt32();
            ViewFirstSlice = loader.ReadUInt32();
            ViewNumSlices = loader.ReadUInt32();
            CompSel = loader.ReadUInt32();
            Regs = loader.ReadUInt32s(5);
            Handle = loader.ReadUInt32();
            ArrayLength = loader.ReadUInt32(); // Possibly just a byte.
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            OfsData = loader.ReadOffset();
            OfsMipData = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
            NumUserData = loader.ReadUInt16();
            loader.Seek(2);
        }
    }
}