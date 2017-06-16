using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    public class ShaderAssign : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string ShaderArchiveName { get; set; }

        public string ShadingModelName { get; set; }

        public uint Revision { get; set; }

        public IList<string> AttribAssigns { get; private set; }

        public IList<string> SamplerAssigns { get; private set; }

        public IList<string> ShaderOptions { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShaderArchiveName = loader.LoadString();
            ShadingModelName = loader.LoadString();
            Revision = loader.ReadUInt32();
            byte numAttribAssign = loader.ReadByte();
            byte numSamplerAssign = loader.ReadByte();
            ushort numShaderOption = loader.ReadUInt16();
            AttribAssigns = loader.LoadDictNames();
            SamplerAssigns = loader.LoadDictNames();
            ShaderOptions = loader.LoadDictNames();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
        }
    }
}