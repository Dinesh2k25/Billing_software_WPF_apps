using PdfSharp;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS
{
    
        public class SimpleFontResolver : IFontResolver
        {
            public string DefaultFontName => "Arial";

            public byte[] GetFont(string faceName)
            {
                // We're not embedding fonts, just returning null to use system font
                return null;
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                // Always use Arial as fallback
                return new FontResolverInfo("Arial");
            }
        
    }
}
