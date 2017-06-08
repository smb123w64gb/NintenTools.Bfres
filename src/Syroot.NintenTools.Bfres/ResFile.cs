using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a NintendoWare for Cafe (NW4F) graphics data archive file.
    /// </summary>
    [DebuggerDisplay(nameof(ResFile) + " {" + nameof(Name) + "}")]
    public class ResFile : IResData
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFile"/> class.
        /// </summary>
        public ResFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFile"/> class from the given <paramref name="stream"/> which
        /// is optionally left open.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        public ResFile(Stream stream, bool leaveOpen = false)
        {
            ResFileLoader loader = new ResFileLoader(this, stream, leaveOpen);
            loader.Execute();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFile"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public ResFile(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the revision of the BFRES structure formats.
        /// </summary>
        public uint Version { get; private set; }

        /// <summary>
        /// Gets the byte order in which data is stored.
        /// </summary>
        public ByteOrder ByteOrder { get; private set; }

        /// <summary>
        /// Gets or sets a name describing the contents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the stored <see cref="Model"/> (FMDL) instances.
        /// </summary>
        public INamedResDataList<Model> Models { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="Texture"/> (FTEX) instances.
        /// </summary>
        public INamedResDataList<Texture> Textures { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="SkeletalAnim"/> (FSKA) instances.
        /// </summary>
        public INamedResDataList<SkeletalAnim> SkeletalAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="ShaderParamAnim"/> (FSHU) instances.
        /// </summary>
        public INamedResDataList<ShaderParamAnim> ShaderParamAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="ShaderParamAnim"/> (FSHU) instances for color animations.
        /// </summary>
        public INamedResDataList<ShaderParamAnim> ColorAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="ShaderParamAnim"/> (FSHU) instances for texture SRT animations.
        /// </summary>
        public INamedResDataList<ShaderParamAnim> TexSrtAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="TexPatternAnim"/> (FTXP) instances.
        /// </summary>
        public INamedResDataList<TexPatternAnim> TexPatternAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="VisibilityAnim"/> (FVIS) instances for bone visibility animations.
        /// </summary>
        public INamedResDataList<VisibilityAnim> BoneVisibilityAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="VisibilityAnim"/> (FVIS) instances for material visibility animations.
        /// </summary>
        public INamedResDataList<VisibilityAnim> MatVisibilityAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="ShapeAnim"/> (FSHA) instances.
        /// </summary>
        public INamedResDataList<ShapeAnim> ShapeAnims { get; private set; }

        /// <summary>
        /// Gets the stored <see cref="SceneAnim"/> (FSCN) instances.
        /// </summary>
        public INamedResDataList<SceneAnim> SceneAnims { get; private set; }

        /// <summary>
        /// Gets attached <see cref="ExternalFile"/> instances. The key of the dictionary typically represents the name
        /// of the file they were originally created from.
        /// </summary>
        public IDictionary<string, ExternalFile> ExternalFiles { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ResFileHead head = new ResFileHead(loader);
            Version = head.Version;
            ByteOrder = head.ByteOrder;
            Name = loader.GetName(head.OfsName);
            Models = loader.LoadDictList<Model>(head.OfsModelDict);
            Textures = loader.LoadDictList<Texture>(head.OfsTextureDict);
            SkeletalAnims = loader.LoadDictList<SkeletalAnim>(head.OfsSkeletalAnimDict);
            ShaderParamAnims = loader.LoadDictList<ShaderParamAnim>(head.OfsShaderParamDict);
            ColorAnims = loader.LoadDictList<ShaderParamAnim>(head.OfsShaderParamDict);
            TexSrtAnims = loader.LoadDictList<ShaderParamAnim>(head.OfsShaderParamDict);
            TexPatternAnims = loader.LoadDictList<TexPatternAnim>(head.OfsTexPatternAnimDict);
            BoneVisibilityAnims = loader.LoadDictList<VisibilityAnim>(head.OfsBoneVisibilityAnimDict);
            MatVisibilityAnims = loader.LoadDictList<VisibilityAnim>(head.OfsMatVisibilityAnimDict);
            ShapeAnims = loader.LoadDictList<ShapeAnim>(head.OfsShapeAnimDict);
            SceneAnims = loader.LoadDictList<SceneAnim>(head.OfsSceneAnimDict);
            ExternalFiles = loader.LoadDict<ExternalFile>(head.OfsExternalFileDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="ResFile"/> instance.
    /// </summary>
    internal class ResFileHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FRES";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint Version;
        internal ByteOrder ByteOrder;
        internal ushort SizHeader;
        internal uint SizFile;
        internal uint Alignment;
        internal uint OfsName;
        internal uint SizeStringPool;
        internal uint OfsStringPool;
        internal uint OfsModelDict;
        internal uint OfsTextureDict;
        internal uint OfsSkeletalAnimDict;
        internal uint OfsShaderParamDict;
        internal uint OfsColorAnimDict;
        internal uint OfsTexSrtAnimDict;
        internal uint OfsTexPatternAnimDict;
        internal uint OfsBoneVisibilityAnimDict;
        internal uint OfsMatVisibilityAnimDict;
        internal uint OfsShapeAnimDict;
        internal uint OfsSceneAnimDict;
        internal uint OfsExternalFileDict;
        internal ushort NumModel;
        internal ushort NumTexture;
        internal ushort NumSkeletalAnim;
        internal ushort NumShaderParam;
        internal ushort NumColorAnim;
        internal ushort NumTexSrtAnim;
        internal ushort NumTexPatternAnim;
        internal ushort NumBoneVisibilityAnim;
        internal ushort NumMatVisibilityAnim;
        internal ushort NumShapeAnim;
        internal ushort NumSceneAnim;
        internal ushort NumExternalFile;
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ResFileHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            Version = loader.ReadUInt32();
            ByteOrder = loader.ReadEnum<ByteOrder>(true);
            SizHeader = loader.ReadUInt16();
            SizFile = loader.ReadUInt32();
            Alignment = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            SizeStringPool = loader.ReadUInt32();
            OfsStringPool = loader.ReadOffset();
            OfsModelDict = loader.ReadOffset();
            OfsTextureDict = loader.ReadOffset();
            OfsSkeletalAnimDict = loader.ReadOffset();
            OfsShaderParamDict = loader.ReadOffset();
            OfsColorAnimDict = loader.ReadOffset();
            OfsTexSrtAnimDict = loader.ReadOffset();
            OfsTexPatternAnimDict = loader.ReadOffset();
            OfsBoneVisibilityAnimDict = loader.ReadOffset();
            OfsMatVisibilityAnimDict = loader.ReadOffset();
            OfsShapeAnimDict = loader.ReadOffset();
            OfsSceneAnimDict = loader.ReadOffset();
            OfsExternalFileDict = loader.ReadOffset();
            NumModel = loader.ReadUInt16();
            NumTexture = loader.ReadUInt16();
            NumSkeletalAnim = loader.ReadUInt16();
            NumShaderParam = loader.ReadUInt16();
            NumColorAnim = loader.ReadUInt16();
            NumTexSrtAnim = loader.ReadUInt16();
            NumTexPatternAnim = loader.ReadUInt16();
            NumBoneVisibilityAnim = loader.ReadUInt16();
            NumMatVisibilityAnim = loader.ReadUInt16();
            NumShapeAnim = loader.ReadUInt16();
            NumSceneAnim = loader.ReadUInt16();
            NumExternalFile = loader.ReadUInt16();
            UserPointer = loader.ReadOffset();
        }
    }
}
