using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Syroot.Maths;
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

        public sbyte DistanceAttenuationFuncIndex { get; set; }

        public sbyte AngleAttenuationFuncIndex { get; set; }

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

        public string DistanceAttenuationFuncName { get; set; }

        public string AngleAttenuationFuncName { get; set; }

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
            DistanceAttenuationFuncIndex = head.IdxDistanceAttenuationFunc;
            AngleAttenuationFuncIndex = head.IdxAngleAttenuationFunc;
            BakedSize = head.SizBaked;
            Name = loader.GetName(head.OfsName);
            LightTypeName = loader.GetName(head.OfsLightTypeName);
            DistanceAttenuationFuncName = loader.GetName(head.OfsDistAttenuationFuncName);
            AngleAttenuationFuncName = loader.GetName(head.OfsAngleAttenuationFuncName);
            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

            loader.Position = head.OfsResult;
            BaseData = new LightAnimData(Flags);
            ((IResData)BaseData).Load(loader);

            UserData = loader.LoadNamedDictList<UserData>(head.OfsUserDataDict);
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
        internal sbyte IdxDistanceAttenuationFunc;
        internal sbyte IdxAngleAttenuationFunc;
        internal uint SizBaked;
        internal uint OfsName;
        internal uint OfsLightTypeName;
        internal uint OfsDistAttenuationFuncName;
        internal uint OfsAngleAttenuationFuncName;
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
            IdxDistanceAttenuationFunc = loader.ReadSByte();
            IdxAngleAttenuationFunc = loader.ReadSByte();
            SizBaked = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            OfsLightTypeName = loader.ReadOffset();
            OfsDistAttenuationFuncName = loader.ReadOffset();
            OfsAngleAttenuationFuncName = loader.ReadOffset();
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
        ResultDistanceAttenuation = 1 << 12,
        ResultAngleAttenuation =  1 << 13,
        ResultColor0 = 1 << 14,
        ResultColor1 = 1 << 15
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LightAnimData : IResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private LightAnimFlags _flags;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal LightAnimData(LightAnimFlags flags)
        {
            _flags = flags;
            Enable = 0;
            Position = Vector3F.Zero;
            Rotation = Vector3F.Zero;
            DistanceAttenuation = Vector2F.Zero;
            AngleAttenuation = Vector2F.Zero;
            Color0 = Vector3F.Zero;
            Color1 = Vector3F.Zero;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public int Enable { get; set; }

        public Vector3F Position { get; set; }

        public Vector3F Rotation { get; set; }

        public Vector2F DistanceAttenuation { get; set; }

        public Vector2F AngleAttenuation { get; set; }

        public Vector3F Color0 { get; set; }

        public Vector3F Color1 { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            if (_flags.HasFlag(LightAnimFlags.ResultEnable)) Enable = loader.ReadInt32();
            if (_flags.HasFlag(LightAnimFlags.ResultPosition)) Position = loader.ReadVector3F();
            if (_flags.HasFlag(LightAnimFlags.ResultRotation)) Rotation = loader.ReadVector3F();
            if (_flags.HasFlag(LightAnimFlags.ResultDistanceAttenuation)) DistanceAttenuation = loader.ReadVector2F();
            if (_flags.HasFlag(LightAnimFlags.ResultAngleAttenuation)) AngleAttenuation = loader.ReadVector2F();
            if (_flags.HasFlag(LightAnimFlags.ResultColor0)) Color0 = loader.ReadVector3F();
            if (_flags.HasFlag(LightAnimFlags.ResultColor1)) Color1 = loader.ReadVector3F();
        }
    }

    public enum LightAnimDataOffset : uint
    {
        Enable = 0x00,
        PositionX = 0x04,
        PositionY = 0x08,
        PositionZ = 0x0C,
        RotationX = 0x10,
        RotationY = 0x14,
        RotationZ = 0x18,
        DistanceAttenuationX = 0x1C,
        DistanceAttenuationY = 0x20,
        AngleAttenuationX = 0x24,
        AngleAttenuationY = 0x28,
        Color0R = 0x2C,
        Color0G = 0x30,
        Color0B = 0x34,
        Color1R = 0x38,
        Color1G = 0x3C,
        Color1B = 0x40
    }
}