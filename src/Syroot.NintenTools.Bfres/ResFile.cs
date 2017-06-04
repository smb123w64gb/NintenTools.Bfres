using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a NintendoWare for Wii U (NW4F) graphics data archive file.
    /// </summary>
    [DebuggerDisplay(nameof(ResFile) + " {" + nameof(Name) + "}")]
    public class ResFile : INamedResData
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public ResFile()
        {
        }

        public ResFile(Stream stream, bool leaveOpen = false)
        {
            ResFileLoader loader = new ResFileLoader(this, stream, leaveOpen);
            ((IResData)this).Load(loader);
        }

        public ResFile(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public uint Version { get; private set; } 

        public ByteOrder ByteOrder { get; private set; }

        public string Name { get; set; }

        public INamedResDataList<Model> Models { get; private set; }

        public INamedResDataList<Texture> Textures { get; private set; }

        public INamedResDataList<SkeletalAnim> SkeletalAnims { get; private set; }

        public INamedResDataList<ShaderParamAnim> ShaderParamAnims { get; private set; }

        public INamedResDataList<ShaderParamAnim> ColorAnims { get; private set; }

        public INamedResDataList<ShaderParamAnim> TexSrtAnims { get; private set; }

        public INamedResDataList<TexPatternAnim> TexPatternAnims { get; private set; }

        public INamedResDataList<VisibilityAnim> BoneVisibilityAnims { get; private set; }

        public INamedResDataList<VisibilityAnim> MatVisibilityAnims { get; private set; }

        public INamedResDataList<ShapeAnim> ShapeAnims { get; private set; }

        public INamedResDataList<SceneAnim> SceneAnims { get; private set; }

        public IDictionary<string, ExternalFile> ExternalFiles { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ResFileHead head = new ResFileHead(loader);
            Version = head.Version;
            ByteOrder = head.ByteOrder;
            Name = loader.GetName(head.OfsName);
            Models = loader.LoadNamedDictList<Model>(head.OfsModelDict);
            Textures = loader.LoadNamedDictList<Texture>(head.OfsTextureDict);
            SkeletalAnims = loader.LoadNamedDictList<SkeletalAnim>(head.OfsSkeletalAnimDict);
            ShaderParamAnims = loader.LoadNamedDictList<ShaderParamAnim>(head.OfsShaderParamDict);
            ColorAnims = loader.LoadNamedDictList<ShaderParamAnim>(head.OfsShaderParamDict);
            TexSrtAnims = loader.LoadNamedDictList<ShaderParamAnim>(head.OfsShaderParamDict);
            TexPatternAnims = loader.LoadNamedDictList<TexPatternAnim>(head.OfsTexPatternAnimDict);
            BoneVisibilityAnims = loader.LoadNamedDictList<VisibilityAnim>(head.OfsBoneVisibilityAnimDict);
            MatVisibilityAnims = loader.LoadNamedDictList<VisibilityAnim>(head.OfsMatVisibilityAnimDict);
            ShapeAnims = loader.LoadNamedDictList<ShapeAnim>(head.OfsShapeAnimDict);
            SceneAnims = loader.LoadNamedDictList<SceneAnim>(head.OfsSceneAnimDict);
            ExternalFiles = loader.LoadDict<ExternalFile>(head.OfsExternalFileDict);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="ResFile"/> instance.
    /// </summary>
    internal class ResFileHead
    {
        // ---- CONSTANTS ------------------------------------------------------------------------------------------

        private const string _signature = "FRES";

        // ---- FIELDS ---------------------------------------------------------------------------------------------

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

        // ---- CONSTRUCTORS & DESTRUCTOR --------------------------------------------------------------------------

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
