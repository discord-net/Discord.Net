using System;

namespace Discord.Commands {

  [AttributeUsage(AttributeTargets.Property)]
  public class DontInjectAttribute : Attribute {
  }

}
