using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TheLastOne.GameItemCtrl
{
    public class Game_ItemCtrl
    {
        private int id = -1;
        private string name = "";
        private Vector3 pos;
        private bool eat = false;
        private bool draw = false;
        public GameObject item;

        public int get_id() { return this.id; }
        public string get_name() { return this.name; }
        public Vector3 get_pos() { return this.pos; }
        public bool get_eat() { return this.eat; }
        public bool get_draw() { return this.draw; }

        public void set_id(int value) { this.id = value; }
        public void set_name(string value) { this.name = value; }
        public void set_pos(Vector3 value) { this.pos = value; }
        public void set_eat(bool value) { this.eat = value; }
        public void set_draw(bool value) { this.draw = value; }
        public Game_ItemCtrl(int id, string name, Vector3 pos, bool eat)
        {
            this.id = id;
            this.name = name;
            this.pos = pos;
            this.eat = eat;
        }
    }
}
