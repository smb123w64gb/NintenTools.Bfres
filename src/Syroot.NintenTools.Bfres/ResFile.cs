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
    [DebuggerDisplay(nameof(Model) + " {" + nameof(Name) + "}")]
    public class ResFile : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------
        
        public ResFile(ResFileLoader loader)
            : base(loader)
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

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public static ResFile FromFile(string fileName)
        {
            ResFileLoader loader = new ResFileLoader(fileName);
            return loader.ResFile;
        }

        public static ResFile FromStream(Stream stream, bool leaveOpen = false)
        {
            ResFileLoader loader = new ResFileLoader(stream, leaveOpen);
            return loader.ResFile;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public uint Version { get; }

        public ByteOrder ByteOrder { get; }

        public string Name { get; set; }

        public IList<Model> Models { get; }

        public IList<Texture> Textures { get; }

        public IList<SkeletalAnim> SkeletalAnims { get; }

        public IList<ShaderParamAnim> ShaderParamAnims { get; }

        public IList<ShaderParamAnim> ColorAnims { get; }

        public IList<ShaderParamAnim> TexSrtAnims { get; }

        public IList<TexPatternAnim> TexPatternAnims { get; }

        public IList<VisibilityAnim> BoneVisibilityAnims { get; }

        public IList<VisibilityAnim> MatVisibilityAnims { get; }

        public IList<ShapeAnim> ShapeAnims { get; }

        public IList<SceneAnim> SceneAnims { get; }

        public IDictionary<string, ExternalFile> ExternalFiles { get; }
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
