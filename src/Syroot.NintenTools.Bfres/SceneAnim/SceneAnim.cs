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
    public class SceneAnim : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

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

        public INamedResDataList<CameraAnim> CameraAnims { get; private set; }

        public INamedResDataList<LightAnim> LightAnims { get; private set; }
        
        public INamedResDataList<FogAnim> FogAnims { get; private set; }

        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            SceneAnimHead head = new SceneAnimHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            CameraAnims = loader.LoadDictList<CameraAnim>(head.OfsCameraAnimDict);
            LightAnims = loader.LoadDictList<LightAnim>(head.OfsLightAnimDict);
            FogAnims = loader.LoadDictList<FogAnim>(head.OfsFogAnimDict);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
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