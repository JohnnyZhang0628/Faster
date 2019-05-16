using System;
using System.Collections.Generic;
using System.Text;
using Faster;

namespace Model
{
  public class TB_ROLE_BUTTON
    {
        [FasterKey]
        public int ROLE_ID { get; set; }
        [FasterKey]
        public int MENU_ID { get; set; }
        [FasterKey]
        public int BTN_ID { get; set; }
    }
}
