using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FCAM section in a <see cref="SceneAnim"/> subfile, storing animations controlling fog settings.
    /// </summary>
    [DebuggerDisplay(nameof(FogAnim) + " {" + nameof(Name) + "}")]
    public class FogAnim : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public FogAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public sbyte DistanceAttenuationFuncIndex { get; set; }

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

        public string DistanceAttenuationFuncName { get; set; }

        public IList<AnimCurve> Curves { get; private set; }

        public FogAnimData BaseData { get; set; }

        public INamedResDataList<UserData> UserData { get; private set; }
        
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            FogAnimHead head = new FogAnimHead(loader);
            Flags = head.Flags;
            FrameCount = head.NumFrame;
            DistanceAttenuationFuncIndex = head.IdxDistanceAttenuationFunc;
            BakedSize = head.SizBaked;
            Name = loader.GetName(head.OfsName);
            DistanceAttenuationFuncName = loader.GetName(head.OfsDistanceAttenuationFuncName);
            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);
            BaseData = loader.Load<FogAnimData>(head.OfsBaseData);
            UserData = loader.LoadNamedDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
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
        internal uint OfsBaseData;
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
            OfsBaseData = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum FogAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct FogAnimData : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public Vector2F DistanceAttenuation { get; set; }
        
        public Vector3F Color { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            DistanceAttenuation = loader.ReadVector2F();
            Color = loader.ReadVector3F();
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    public enum FogAnimDataOffset : uint
    {
        DistanceAttenuationX = 0x00,
        DistanceAttenuationY = 0x04,
        ColorR = 0x08,
        ColorG = 0x0C,
        ColorB = 0x10
    }
}