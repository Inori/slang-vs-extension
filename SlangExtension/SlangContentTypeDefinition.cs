using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlangExtension
{
    public class SlangContentDefinition
    {
        [Export]
        [Name("slang")]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition SlangContentTypeDefinition;


        [Export]
        [FileExtension(".slang")]
        [ContentType("slang")]
        internal static FileExtensionToContentTypeDefinition SlangFileExtensionDefinition;
    }
}
