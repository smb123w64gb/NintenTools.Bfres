using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FLIT section in a <see cref="SceneAnim"/> subfile, storing animations controlling light settings.
    /// </summary>
    [DebuggerDisplay(nameof(LightAnim) + " {" + nameof(Name) + "}")]
    public class LightAnim : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FLIT";
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets flags controlling how animation data is stored or how the animation should be played.
        /// </summary>
        public LightAnimFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the total number of frames this animation plays.
        /// </summary>
        public int FrameCount { get; set; }

        public sbyte LightTypeIndex { get; set; }

        public sbyte DistanceAttnFuncIndex { get; set; }

        public sbyte AngleAttnFuncIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes required to bake all <see cref="Curves"/>.
        /// </summary>
        public uint BakedSize { get; private set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in <see cref="ResDict{LightAnim}"/>
        /// instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the light type.
        /// </summary>
        public string LightTypeName { get; set; }

        public string DistanceAttnFuncName { get; set; }

        public string AngleAttnFuncName { get; set; }

        /// <summary>
        /// Gets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; private set; }

        public LightAnimData BaseData { get; set; }

        /// <summary>
        /// Gets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public ResDict<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Flags = loader.ReadEnum<LightAnimFlags>(true);
            ushort numUserData = loader.ReadUInt16();
            FrameCount = loader.ReadInt32();
            byte numCurve = loader.ReadByte();
            LightTypeIndex = loader.ReadSByte();
            DistanceAttnFuncIndex = loader.ReadSByte();
            AngleAttnFuncIndex = loader.ReadSByte();
            BakedSize = loader.ReadUInt32();
            Name = loader.LoadString();
            LightTypeName = loader.LoadString();
            DistanceAttnFuncName = loader.LoadString();
            AngleAttnFuncName = loader.LoadString();
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseData = loader.LoadCustom(() => new LightAnimData(loader, Flags));
            UserData = loader.LoadDict<UserData>();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.Write(Flags, true);
            saver.Write((ushort)UserData.Count);
            saver.Write(FrameCount);
            saver.Write((byte)Curves.Count);
            saver.Write(LightTypeIndex);
            saver.Write(DistanceAttnFuncIndex);
            saver.Write(AngleAttnFuncIndex);
            saver.Write(BakedSize);
            saver.SaveString(Name);
            saver.SaveString(LightTypeName);
            saver.SaveString(DistanceAttnFuncName);
            saver.SaveString(AngleAttnFuncName);
            saver.SaveList(Curves);
            saver.SaveCustom(BaseData, () => BaseData.Save(saver, Flags));
            saver.SaveDict(UserData);
        }
    }
    
    /// <summary>
    /// Represents flags specifying how animation data is stored or should be played.
    /// </summary>
    [Flags]
    public enum LightAnimFlags : ushort
    {
        /// <summary>
        /// The stored curve data has been baked.
        /// </summary>
        BakedCurve = 1 << 0,

        /// <summary>
        /// The animation repeats from the start after the last frame has been played.
        /// </summary>
        Looping = 1 << 2,

        EnableCurve = 1 << 8,
        ResultEnable = 1 << 9,
        ResultPosition = 1 << 10,
        ResultRotation = 1 << 11,
        ResultDistanceAttn = 1 << 12,
        ResultAngleAttn =  1 << 13,
        ResultColor0 = 1 << 14,
        ResultColor1 = 1 << 15
    }
}