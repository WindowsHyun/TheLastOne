//------------------------------------------------------------------------------
            // 위치 이동 치트.
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(349.1992f, 60.06981f, 376.0149f);
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
                tr.transform.position = new Vector3(526.118f, 50.04224f, 2705.361f);
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
                tr.transform.position = new Vector3(1545.017f, 59.95751f, 904.9451f);
                navagent.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                navagent.enabled = false;
                tr.transform.position = new Vector3(1008.397f, 30.06981f, 1553.832f);
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