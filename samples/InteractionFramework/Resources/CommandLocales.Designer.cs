namespace DiscordNetTestBot.Properties {
    using System;   
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CommandLocales
    {
    
        private static global::System.Resources.ResourceManager resourceMan;
    
        private static global::System.Globalization.CultureInfo resourceCulture;
    
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommandLocales()
        {
        }
    
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CommandLocales", typeof(CommandLocales).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
    
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }
    
        internal static string echo_description
        {
            get
            {
                return ResourceManager.GetString("echo.description", resourceCulture);
            }
        }
    
        internal static string echo_mention_description
        {
            get
            {
                return ResourceManager.GetString("echo.mention.description", resourceCulture);
            }
        }
    
        internal static string echo_name
        {
            get
            {
                return ResourceManager.GetString("echo.name", resourceCulture);
            }
        }
    }
}

