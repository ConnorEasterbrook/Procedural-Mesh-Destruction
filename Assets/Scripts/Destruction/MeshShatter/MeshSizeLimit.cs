/**
 * Copyright 2022 Connor Easterbrook
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Connoreaster
{
    public class MeshSizeLimit : MonoBehaviour
    {
        private float minShapeSize = 0.05f; // Set the minimum size of the shape
        public bool isPlane = false;

        // Update is called once per frame
        void Update()
        {
            Mesh gameObjectMesh = GetComponent<MeshFilter>().mesh; // Get the mesh of the game object

            // Check if the object is a plane or not to shatter properly
            if (!isPlane)
            {
                if (gameObjectMesh.bounds.size.x * gameObjectMesh.bounds.size.y * gameObjectMesh.bounds.size.z < minShapeSize / 100)
                {
                    Debug.Log("Mesh is too small, destroying");
                    Destroy(gameObject);
                }
            }
            else
            {
                if (gameObjectMesh.bounds.size.x * gameObjectMesh.bounds.size.z < minShapeSize)
                {
                    Debug.Log("Mesh is too small, destroying");
                    Destroy(gameObject);
                }
            }
        }
    }
}
