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

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public LightAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public sbyte LightTypeIndex { get; set; }

        public sbyte DistanceAttnFuncIndex { get; set; }

        public sbyte AngleAttnFuncIndex { get; set; }

        public uint BakedSize { get; private set; }

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

        public IList<AnimCurve> Curves { get; private set; }

        public LightAnimData BaseData { get; set; }

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

    [Flags]
    public enum LightAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
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