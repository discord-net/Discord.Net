using System;

namespace Discord.Commands {

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DontInjectAttribute : Attribute {
  }

}
