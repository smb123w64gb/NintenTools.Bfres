using System;
using System.Collections.Generic;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FCAM section in a <see cref="SceneAnim"/> subfile, storing animations controlling fog settings.
    /// </summary>
    public class FogAnim : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public FogAnim(ResFileLoader loader)
            : base(loader)
        {
            FogAnimHead head = new FogAnimHead(loader);
            Flags = head.Flags;
            FrameCount = head.NumFrame;
            DistanceAttenuationFuncIndex = head.IdxDistanceAttenuationFunc;
            BakedSize = head.SizBaked;
            Name = loader.GetName(head.OfsName);
            DistanceAttenuationFuncName = loader.GetName(head.OfsDistanceAttenuationFuncName);
            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);
            Result = loader.Load<FogAnimResult>(head.OfsResult);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public FogAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public sbyte DistanceAttenuationFuncIndex { get; set; }

        public uint BakedSize { get; }

        public string Name { get; set; }

        public string DistanceAttenuationFuncName { get; set; }

        public IList<AnimCurve> Curves { get; }

        public FogAnimResult Result { get; set; }

        public IList<UserData> UserData { get; }
    }

    /// <summary>
    /// Represents the header of a <see cref="FogAnim"/> instance.
    /// </summary>
    internal class FogAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FFOG";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal FogAnimFlags Flags;
        internal int NumFrame;
        internal byte NumCurve;
        internal sbyte IdxDistanceAttenuationFunc;
        internal ushort NumUserData;
        internal uint SizBaked;
        internal uint OfsName;
        internal uint OfsDistanceAttenuationFuncName;
        internal uint OfsCurveList;
        internal uint OfsResult;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public FogAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            Flags = loader.ReadEnum<FogAnimFlags>(true);
            NumFrame = loader.ReadInt32();
            NumCurve = loader.ReadByte();
            IdxDistanceAttenuationFunc = loader.ReadSByte();
            NumUserData = loader.ReadUInt16();
            SizBaked = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            OfsDistanceAttenuationFuncName = loader.ReadOffset();
            OfsCurveList = loader.ReadOffset();
            OfsResult = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum FogAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }

    public class FogAnimResult : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public FogAnimResult(ResFileLoader loader)
            : base(loader)
        {
            DistanceAttenuation = loader.ReadVector2F();
            Color = loader.ReadVector3F();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public Vector2F DistanceAttenuation { get; set; }
        
        public Vector3F Color { get; set; }
    }
}