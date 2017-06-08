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
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public FogAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public sbyte DistanceAttnFuncIndex { get; set; }

        public uint BakedSize { get; private set; }

        /// <summary>
        /// The name with which the instance can be referenced uniquely in <see cref="INamedResDataList{FogAnim}"/>
        /// instances.
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

        public IList<AnimCurve> Curves { get; private set; }

        public FogAnimData BaseData { get; set; }

        public INamedResDataList<UserData> UserData { get; private set; }
        
        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            FogAnimHead head = new FogAnimHead(loader);
            Flags = head.Flags;
            FrameCount = head.NumFrame;
            DistanceAttnFuncIndex = head.IdxDistanceAttnFunc;
            BakedSize = head.SizBaked;
            Name = loader.GetName(head.OfsName);
            DistanceAttnFuncName = loader.GetName(head.OfsDistanceAttnFuncName);
            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

            if (head.OfsBaseData != 0)
            {
                loader.Position = head.OfsBaseData;
                BaseData = new FogAnimData(loader);
            }

            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
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
        internal sbyte IdxDistanceAttnFunc;
        internal ushort NumUserData;
        internal uint SizBaked;
        internal uint OfsName;
        internal uint OfsDistanceAttnFuncName;
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
            IdxDistanceAttnFunc = loader.ReadSByte();
            NumUserData = loader.ReadUInt16();
            SizBaked = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            OfsDistanceAttnFuncName = loader.ReadOffset();
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
}