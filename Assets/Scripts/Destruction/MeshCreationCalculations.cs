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
    public class MeshCreationCalculations
    {
        private MeshCutCalculations calcScript;
        private Mesh completeMesh1;
        private Mesh completeMesh2;
        private GameObject secondMeshGO;

        public MeshCreationCalculations(MeshCutCalculations _calcScript, Mesh mesh1, Mesh mesh2, GameObject _secondMeshGO)
        {
            calcScript = _calcScript;
            completeMesh1 = mesh1;
            completeMesh2 = mesh2;
            secondMeshGO = _secondMeshGO;
        }

        public GameObject CallScript(GameObject _sentGameObject)
        {
            CreateFirstMesh(_sentGameObject);

            return secondMeshGO;
        }

        /// <summary>
        /// Create the new meshes after the slicing
        /// </summary>
        private void CreateFirstMesh(GameObject hitGameObject)
        {
            // Remove all current colliders on the object to avoid duplicates and update the mesh bounds
            Collider[] originalCols = hitGameObject.GetComponents<Collider>();
            foreach (Collider col in originalCols)
            {
                MonoBehaviour.Destroy(col);
            }

            hitGameObject.GetComponent<MeshFilter>().mesh = completeMesh1; // Set the first object's mesh to the first mesh
            var collider = hitGameObject.AddComponent<MeshCollider>(); // Add a mesh collider to the first object
            collider.sharedMesh = completeMesh1; // Set the collider's mesh to the first mesh
            collider.convex = true; // Set the collider to convex since convex colliders are faster

            AddMaterial(completeMesh1, hitGameObject, hitGameObject, true); // Add the material to the first object

            if (hitGameObject.GetComponent<MeshSizeLimit>() == null)
            {
                hitGameObject.AddComponent<MeshSizeLimit>(); // Add a mesh size limit script to the second mesh if the original mesh doesn't have one
            }

            CreateSecondMesh(hitGameObject);

            if (completeMesh1.bounds.size.x * completeMesh1.bounds.size.y * completeMesh1.bounds.size.z < 1.75f / 100)
            {
                DeleteAfterSeven(hitGameObject);
            }

            AddRigidBody(hitGameObject, hitGameObject); // Add a rigidbody to the second mesh
        }

        private void CreateSecondMesh(GameObject hitGameObject)
        {
            secondMeshGO = new GameObject(); // Create a new game object for the second mesh
            secondMeshGO.tag = "Sliceable"; // Set the tag to sliceable
            secondMeshGO.transform.position = hitGameObject.transform.position + (Vector3.up * .05f); // Set the position of the second mesh to the position of the first mesh plus a small offset
            secondMeshGO.transform.rotation = hitGameObject.transform.rotation; // Set the rotation of the second mesh to the rotation of the first mesh
            secondMeshGO.transform.localScale = hitGameObject.transform.localScale; // Set the scale of the second mesh to the scale of the first mesh
            secondMeshGO.AddComponent<MeshRenderer>(); // Add a mesh renderer to the second mesh

            if (hitGameObject.GetComponent<MeshSizeLimit>() == null)
            {
                secondMeshGO.AddComponent<MeshSizeLimit>(); // Add a mesh size limit script to the second mesh if the original mesh doesn't have one
            }
            else
            {
                secondMeshGO.AddComponent<MeshSizeLimit>().isPlane = hitGameObject.GetComponent<MeshSizeLimit>().isPlane; // Add a mesh size limit script to the second mesh
            }

            AddMaterial(completeMesh2, hitGameObject, secondMeshGO, false); // Add the material to the second mesh

            secondMeshGO.AddComponent<MeshFilter>().mesh = completeMesh2; // Add a mesh filter to the second mesh and set the mesh to the second mesh

            secondMeshGO.AddComponent<MeshCollider>().sharedMesh = completeMesh2; // Add a mesh collider to the second mesh and set the mesh to the second mesh
            var cols = secondMeshGO.GetComponents<MeshCollider>(); // Get all the mesh colliders on the second mesh and set them to convex since convex colliders are faster
            foreach (var col in cols)
            {
                col.convex = true;

                if (!col.convex)
                {
                    MonoBehaviour.Destroy(col.gameObject);
                }
            }

            AddRigidBody(secondMeshGO, hitGameObject); // Add a rigidbody to the second mesh

            if (hitGameObject.tag == "Limb")
            {
                DismemberHelper _helper = new DismemberHelper(); // Create a new dismember helper
                if (hitGameObject.transform.childCount > 0)
                {
                    _helper.RemoveComponents(hitGameObject); // Remove the components from the hit game object
                }
                else
                {
                    _helper.RemoveComponents(hitGameObject); // Remove the components from the hit game object
                }
            }

            if (completeMesh2.bounds.size.x * completeMesh2.bounds.size.y * completeMesh2.bounds.size.z < 1.75f / 100)
            {
                DeleteAfterSeven(secondMeshGO);
            }
        }

        private void AddMaterial(Mesh mesh, GameObject hitGameObject, GameObject newGameObject, bool isMesh1)
        {
            MeshRenderer meshRenderer = new MeshRenderer(); // Create a new mesh renderer
            isMesh1 = false ? meshRenderer = newGameObject.AddComponent<MeshRenderer>() : meshRenderer = hitGameObject.GetComponent<MeshRenderer>(); // Add a mesh renderer to the new object
            Material[] oldMats = hitGameObject.GetComponent<MeshRenderer>().materials; // Get the materials of the original object
            Material[] newMats = new Material[mesh.subMeshCount]; // Create a new array of materials
            Material innerMat = null;

            if (hitGameObject.GetComponent<InnerMaterial>() != null)
            {
                innerMat = hitGameObject.GetComponent<InnerMaterial>().innerMaterial; // Get the inner material of the original object
            }

            for (int i = 0; i < oldMats.Length; i++)
            {
                if (mesh.subMeshCount != 0)
                {
                    newMats[i] = oldMats[i]; // Set the material to the original material
                }
                else
                {
                    MonoBehaviour.Destroy(newGameObject);
                }
            }

            for (int i = oldMats.Length; i < mesh.subMeshCount; i++)
            {
                if (innerMat != null)
                {
                    newMats[i] = innerMat;
                }
                else
                {
                    newMats[i] = hitGameObject.GetComponent<MeshRenderer>().material;
                }
            }

            // meshRenderer.materials = newMats; // Set the materials to the new array of materials
            newGameObject.AddComponent<InnerMaterial>().innerMaterial = hitGameObject.GetComponent<InnerMaterial>().innerMaterial;
            newGameObject.GetComponent<MeshRenderer>().materials = newMats;
        }

        private void AddRigidBody(GameObject _GO, GameObject hitGameObject)
        {
            if (_GO.GetComponent<Rigidbody>() == null)
            {
                Rigidbody secondGORB = _GO.AddComponent<Rigidbody>(); // Add a rigidbody to the second mesh
                secondGORB.mass = hitGameObject.GetComponent<Rigidbody>().mass; // Set the mass of the second mesh to the mass of the first mesh
                // rightRigidbody.AddRelativeForce(-slicePlane.normal * explodeForce); // Add a force to the second mesh in the opposite direction of the slice plane for effect
            }
            else
            {
                _GO.GetComponent<Rigidbody>().useGravity = true;
                _GO.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        private async void DeleteAfterSeven(GameObject _GO)
        {
            await Task.Delay(7000);
            if (_GO != null)
            {
                MonoBehaviour.Destroy(_GO);
            }
        }
    }
}
