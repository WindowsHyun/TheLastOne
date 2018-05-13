//------------------------------------------------------------------------------
            // 위치 이동 치트.
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(776.9319f, 30.00061f, 441.4027f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(718.0281f, 50.0f, 1235.498f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(484.5258f, 30.00061f, 2534.864f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1407.026f, 50.0f, 2190.113f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1651.698f, 30.00061f, 1714.017f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1523.183f, 60.00122f, 940.6536f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                // Navmesh ON/OFF
                if (navagent.enabled == true)
                {
                    navagent.enabled = false;
                }
                else
                {
                    navagent.enabled = true;
                }
            }
            //------------------------------------------------------------------------------