using System;
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
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSCN";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{SceneAnim}"/> instances.
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
        /// Gets the <see cref="CameraAnim"/> instances.
        /// </summary>
        public INamedResDataList<CameraAnim> CameraAnims { get; private set; }
        
        /// <summary>
        /// Gets the <see cref="LightAnim"/> instances.
        /// </summary>
        public INamedResDataList<LightAnim> LightAnims { get; private set; }

        /// <summary>
        /// Gets the <see cref="FogAnim"/> instances.
        /// </summary>
        public INamedResDataList<FogAnim> FogAnims { get; private set; }

        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Name = loader.LoadString();
            Path = loader.LoadString();
            ushort numUserData = loader.ReadUInt16();
            ushort numCameraAnim = loader.ReadUInt16();
            ushort numLightAnim = loader.ReadUInt16();
            ushort numFogAnim = loader.ReadUInt16();
            CameraAnims = loader.LoadDictList<CameraAnim>();
            LightAnims = loader.LoadDictList<LightAnim>();
            FogAnims = loader.LoadDictList<FogAnim>();
            UserData = loader.LoadDictList<UserData>();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveString(Name);
            saver.SaveString(Path);
            saver.Write((ushort)UserData.Count);
            saver.Write((ushort)CameraAnims.Count);
            saver.Write((ushort)LightAnims.Count);
            saver.Write((ushort)FogAnims.Count);
            saver.SaveDictList(CameraAnims);
            saver.SaveDictList(LightAnims);
            saver.SaveDictList(FogAnims);
            saver.SaveDictList(UserData);
        }
    }
}