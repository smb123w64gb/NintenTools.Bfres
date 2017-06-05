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

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public GX2SurfaceDim Dim { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public uint Depth { get; set; }
        
        public uint MipCount { get; set; }

        public GX2SurfaceFormat Format { get; set; }

        public GX2AAMode AAMode { get; set; }

        public GX2SurfaceUse Use { get; set; }

        public GX2TileMode TileMode { get; set; }

        public uint Swizzle { get; set; }

        public uint Alignment { get; set; }

        public uint Pitch { get; set; }

        public uint[] MipOffsets { get; set; }

        public uint ViewMipFirst { get; set; }

        public uint ViewMipCount { get; set; }

        public uint ViewSliceFirst { get; set; }

        public uint ViewSliceCount { get; set; }

        public uint CompSel { get; set; }

        public uint[] Regs { get; set; }
        
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

        public string Path { get; set; }

        public byte[] Data { get; set; }

        public byte[] MipData { get; set; }

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

            UserData = loader.LoadNamedDictList<UserData>(head.OfsUserDataDict);
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