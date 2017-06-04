using System;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    public class KeyShape : IResContent
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public byte[] TargetAttribIndices { get; set; }

        public byte[] TargetAttribIndexOffsets { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResContent.Load(ResFileLoader loader)
        {
            TargetAttribIndices = loader.ReadBytes(20);
            TargetAttribIndexOffsets = loader.ReadBytes(4);
        }
    }
}