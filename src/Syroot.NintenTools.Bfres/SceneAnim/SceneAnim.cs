using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSCN subfile in a <see cref="ResFile"/>, storing scene animations controlling camera, light and
    /// fog settings.
    /// </summary>
    [DebuggerDisplay(nameof(SceneAnim) + " {" + nameof(Name) + "}")]
    public class SceneAnim : IResContent
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public string Path { get; set; }

        public IList<CameraAnim> CameraAnims { get; private set; }

        public IList<LightAnim> LightAnims { get; private set; }
        
        public IList<FogAnim> FogAnims { get; private set; }

        public IList<UserData> UserData { get; private set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Load(ResFileLoader loader)
        {
            SceneAnimHead head = new SceneAnimHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            CameraAnims = loader.LoadDictList<CameraAnim>(head.OfsCameraAnimDict);
            LightAnims = loader.LoadDictList<LightAnim>(head.OfsLightAnimDict);
            FogAnims = loader.LoadDictList<FogAnim>(head.OfsFogAnimDict);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="SceneAnim"/> instance.
    /// </summary>
    internal class SceneAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSCN";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal ushort NumUserData;
        internal ushort NumCameraAnim;
        internal ushort NumLightAnim;
        internal ushort NumFogAnim;
        internal uint OfsCameraAnimDict;
        internal uint OfsLightAnimDict;
        internal uint OfsFogAnimDict;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal SceneAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            NumUserData = loader.ReadUInt16();
            NumCameraAnim = loader.ReadUInt16();
            NumLightAnim = loader.ReadUInt16();
            NumFogAnim = loader.ReadUInt16();
            OfsCameraAnimDict = loader.ReadOffset();
            OfsLightAnimDict = loader.ReadOffset();
            OfsFogAnimDict = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }
}