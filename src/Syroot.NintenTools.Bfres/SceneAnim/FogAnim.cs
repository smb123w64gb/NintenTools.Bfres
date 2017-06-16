using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FCAM section in a <see cref="SceneAnim"/> subfile, storing animations controlling fog settings.
    /// </summary>
    [DebuggerDisplay(nameof(FogAnim) + " {" + nameof(Name) + "}")]
    public class FogAnim : INamedResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FFOG";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets flags controlling how animation data is stored or how the animation should be played.
        /// </summary>
        public FogAnimFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the total number of frames this animation plays.
        /// </summary>
        public int FrameCount { get; set; }

        public sbyte DistanceAttnFuncIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes required to bake all <see cref="Curves"/>.
        /// </summary>
        public uint BakedSize { get; private set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{FogAnim}"/> instances.
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

        public string DistanceAttnFuncName { get; set; }

        /// <summary>
        /// Gets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; private set; }

        public FogAnimData BaseData { get; set; }

        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public INamedResDataList<UserData> UserData { get; private set; }
        
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Flags = loader.ReadEnum<FogAnimFlags>(true);
            FrameCount = loader.ReadInt32();
            byte numCurve = loader.ReadByte();
            DistanceAttnFuncIndex = loader.ReadSByte();
            ushort numUserData = loader.ReadUInt16();
            BakedSize = loader.ReadUInt32();
            Name = loader.LoadString();
            DistanceAttnFuncName = loader.LoadString();
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseData = loader.LoadCustom(() => new FogAnimData(loader));
            UserData = loader.LoadDictList<UserData>();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.Write(Flags, true);
            saver.Write(FrameCount);
            saver.Write((byte)Curves.Count);
            saver.Write(DistanceAttnFuncIndex);
            saver.Write((ushort)UserData.Count);
            saver.Write(BakedSize);
            saver.SaveString(Name);
            saver.SaveString(DistanceAttnFuncName);
            saver.SaveList(Curves);
            saver.SaveCustom(BaseData, () => BaseData.Save(saver));
            saver.SaveDictList(UserData);
        }
    }
    
    /// <summary>
    /// Represents flags specifying how animation data is stored or should be played.
    /// </summary>
    [Flags]
    public enum FogAnimFlags : ushort
    {
        /// <summary>
        /// The stored curve data has been baked.
        /// </summary>
        BakedCurve = 1 << 0,
        
        /// <summary>
        /// The animation repeats from the start after the last frame has been played.
        /// </summary>
        Looping = 1 << 2
    }
}