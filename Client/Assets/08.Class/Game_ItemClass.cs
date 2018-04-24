using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TheLastOne.GameClass
{
    public class Game_ItemClass
    {
        private int id = -1;
        private string name = "";
        private Vector3 pos;
        private Vector3 rotation;
        private bool eat = false;
        private bool draw = false;
        private bool sendPacket = false;
        private int kind = -1;
        public GameObject item;

        public int get_id() { return this.id; }
        public string get_name() { return this.name; }
        public Vector3 get_pos() { return this.pos; }
        public Vector3 get_rotation() { return this.rotation; }
        public bool get_eat() { return this.eat; }
        public bool get_draw() { return this.draw; }
        public bool get_sendPacket() { return this.sendPacket; }
        public int get_kind() { return this.kind; }

        public void set_id(int value) { this.id = value; }
        public void set_name(string value) { this.name = value; }
        public void set_pos(Vector3 value) { this.pos = value; }
        public void set_rotation(Vector3 value) { this.rotation = value; }
        public void set_eat(bool value) { this.eat = value; }
        public void set_draw(bool value) { this.draw = value; }
        public void set_sendPacket(bool value) { this.sendPacket = value; }

        public Game_ItemClass(int id, string name, Vector3 pos, Vector3 rotation, bool eat, int kind)
        {
            this.id = id;
            this.name = name;
            this.pos = pos;
            this.rotation = rotation;
            this.eat = eat;
            this.kind = kind;
        }
    }
}
