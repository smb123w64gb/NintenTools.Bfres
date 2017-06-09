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
    public class LightAnim : INamedResData
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
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="INamedResDataList{LightAnim}"/> instances.
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
        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            LightAnimHead head = new LightAnimHead(loader);
            Flags = head.Flags;
            FrameCount = head.NumFrame;
            LightTypeIndex = head.IdxLightType;
            DistanceAttnFuncIndex = head.IdxDistanceAttnFunc;
            AngleAttnFuncIndex = head.IdxAngleAttnFunc;
            BakedSize = head.SizBaked;
            Name = loader.GetName(head.OfsName);
            LightTypeName = loader.GetName(head.OfsLightTypeName);
            DistanceAttnFuncName = loader.GetName(head.OfsDistAttnFuncName);
            AngleAttnFuncName = loader.GetName(head.OfsAngleAttnFuncName);
            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

            if (head.OfsResult != 0)
            {
                loader.Position = head.OfsResult;
                BaseData = new LightAnimData(loader, Flags);
            }

            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="LightAnim"/> instance.
    /// </summary>
    internal class LightAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FLIT";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal LightAnimFlags Flags;
        internal ushort NumUserData;
        internal int NumFrame;
        internal byte NumCurve;
        internal sbyte IdxLightType;
        internal sbyte IdxDistanceAttnFunc;
        internal sbyte IdxAngleAttnFunc;
        internal uint SizBaked;
        internal uint OfsName;
        internal uint OfsLightTypeName;
        internal uint OfsDistAttnFuncName;
        internal uint OfsAngleAttnFuncName;
        internal uint OfsCurveList;
        internal uint OfsResult;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public LightAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            Flags = loader.ReadEnum<LightAnimFlags>(true);
            NumUserData = loader.ReadUInt16();
            NumFrame = loader.ReadInt32();
            NumCurve = loader.ReadByte();
            IdxLightType = loader.ReadSByte();
            IdxDistanceAttnFunc = loader.ReadSByte();
            IdxAngleAttnFunc = loader.ReadSByte();
            SizBaked = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            OfsLightTypeName = loader.ReadOffset();
            OfsDistAttnFuncName = loader.ReadOffset();
            OfsAngleAttnFuncName = loader.ReadOffset();
            OfsCurveList = loader.ReadOffset();
            OfsResult = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
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