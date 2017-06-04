using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FLIT section in a <see cref="SceneAnim"/> subfile, storing animations controlling light settings.
    /// </summary>
    [DebuggerDisplay(nameof(LightAnim) + " {" + nameof(Name) + "}")]
    public class LightAnim : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public LightAnim(ResFileLoader loader)
            : base(loader)
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
            Result = loader.Load<LightAnimResult>(head.OfsResult, Flags);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public LightAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public sbyte LightTypeIndex { get; set; }

        public sbyte DistanceAttenuationFuncIndex { get; set; }

        public sbyte AngleAttenuationFuncIndex { get; set; }

        public uint BakedSize { get; }

        public string Name { get; set; }

        public string LightTypeName { get; set; }

        public string DistanceAttenuationFuncName { get; set; }

        public string AngleAttenuationFuncName { get; set; }

        public IList<AnimCurve> Curves { get; }

        public LightAnimResult Result { get; set; }

        public IList<UserData> UserData { get; }
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

    public class LightAnimResult : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public LightAnimResult(ResFileLoader loader, LightAnimFlags flags)
            : base(loader)
        {
            if (flags.HasFlag(LightAnimFlags.ResultEnable)) Enable = loader.ReadInt32();
            if (flags.HasFlag(LightAnimFlags.ResultPosition)) Position = loader.ReadVector3F();
            if (flags.HasFlag(LightAnimFlags.ResultRotation)) Rotation = loader.ReadVector3F();
            if (flags.HasFlag(LightAnimFlags.ResultDistanceAttenuation)) DistanceAttenuation = loader.ReadVector2F();
            if (flags.HasFlag(LightAnimFlags.ResultAngleAttenuation)) AngleAttenuation = loader.ReadVector2F();
            if (flags.HasFlag(LightAnimFlags.ResultColor0)) Color0 = loader.ReadVector3F();
            if (flags.HasFlag(LightAnimFlags.ResultColor1)) Color1 = loader.ReadVector3F();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public int Enable { get; set; }

        public Vector3F Position { get; set; }

        public Vector3F Rotation { get; set; }

        public Vector2F DistanceAttenuation { get; set; }

        public Vector2F AngleAttenuation { get; set; }

        public Vector3F Color0 { get; set; }

        public Vector3F Color1 { get; set; }
    }
}