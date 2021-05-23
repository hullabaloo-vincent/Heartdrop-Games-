using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_OrthoLOD : MonoBehaviour {

	List<Renderer[]> LOD_Renderer = new List<Renderer[]>();
	List<float> LOD_Distance = new List<float>(); // Distance à laquelle les objets changent de LOD.
	List<float> LOD_Ortho = new List<float>(); // Taille orthographique correspondante.

	public bool Cutoff = true; // Hide the object if it is beyond the maximum distance.
	public int Skip = 4;
	public Camera cameraActive;
	public int niveauQualitéSpécifique = 4;

	protected int _skipIndex = 0;
	protected int _currentLOD = 0;

	void Start()
	{
        cameraActive = Camera.main;
		/*if (!cameraActive)
		{
			cameraActive = Camera.main;
		}*/

		float profondeurDeChamp = cameraActive.farClipPlane - cameraActive.nearClipPlane;
		LODGroup leLODgroup = this.GetComponentInChildren<LODGroup> ();
		float Rcull = leLODgroup.GetLODs () [leLODgroup.lodCount-1].screenRelativeTransitionHeight; //La dernière limite, celle où on efface le sprite, "cull".
			
		foreach (LOD leLOD in leLODgroup.GetLODs()){
			LOD_Renderer.Add (leLOD.renderers);
			foreach (Renderer leRenderer in leLOD.renderers)
			{
				leRenderer.enabled = false;
			}
			LOD_Distance.Add(niveauQualitéSpécifique*QualitySettings.lodBias*Rcull*profondeurDeChamp/leLOD.screenRelativeTransitionHeight);
			LOD_Ortho.Add (niveauQualitéSpécifique*QualitySettings.lodBias*0.5f / leLOD.screenRelativeTransitionHeight); //nota : une taille orthographique de 1 correspond à un demi écran en hauteur. 0.5f pour 100% de l'écran couvert.
		}
		leLODgroup.enabled = false;

		//Le LOD zéro est affiché par défaut.
		foreach (Renderer leRenderer in leLODgroup.GetLODs () [0].renderers)
		{
			leRenderer.enabled = false;
		}
	}

	void Update() {
		// We don't need to update every frame, but we do need to update a few times every second.

		if (_skipIndex < Skip) {
			_skipIndex ++;
			return;
		}
		else {
			_skipIndex -= Skip;
		}

		float distance;
		if (cameraActive.orthographic) //CAS ORTHOGRAPHIQUE
		{
			// Maintenant, seule la taille orthographique de la caméra compte pour afficher les niveaux de détail.
			distance = cameraActive.orthographicSize;
			if (Cutoff)
			{
				if (distance >= LOD_Ortho [(LOD_Ortho.Count - 1)])
				{
					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = false;
					}
					return;
				}
				else
				{
					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = true;
					}
				}
			}

			// See if the LOD needs to be increased.
			if (LOD_Ortho.Count >= (_currentLOD + 1))
			{
				if (distance >= LOD_Ortho [_currentLOD])
				{
					// Change the mesh

					if (LOD_Renderer.Count >= (_currentLOD + 2))
					{
						foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
						{
							leRenderer.enabled = false;
						}

						_currentLOD++;

						foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
						{
							leRenderer.enabled = true;
						}
					}
				}
			}

			// Should it be decreased?

			if (_currentLOD > 0)
			{
				if (distance < LOD_Ortho [(_currentLOD - 1)])
				{
					// Change the mesh
					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = false;
					}

					_currentLOD--;

					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = true;
					}
				}
			}
		}
		else //CAS PERSPECTIVE CONIQUE
		{
			//Je laisse tomber l'évitement des racines carrées
			distance = Vector3.Distance(cameraActive.transform.position, transform.position);

			if (Cutoff)
			{
				if (distance >= LOD_Distance [(LOD_Distance.Count - 1)])
				{
					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = false;
					}
					return;
				}
				else
				{
					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = true;
					}
				}
			}

			// See if the LOD needs to be increased.
			if (LOD_Distance.Count >= (_currentLOD + 1))
			{
				if (distance >= LOD_Distance [_currentLOD])
				{
					// Change the mesh

					if (LOD_Renderer.Count >= (_currentLOD + 2))
					{
						foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
						{
							leRenderer.enabled = false;
						}

						_currentLOD++;

						foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
						{
							leRenderer.enabled = true;
						}
					}
				}
			}

			// Should it be decreased?

			if (_currentLOD > 0)
			{
				if (distance < LOD_Distance [(_currentLOD - 1)])
				{
					// Change the mesh
					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = false;
					}

					_currentLOD--;

					foreach (Renderer leRenderer in LOD_Renderer[_currentLOD])
					{
						leRenderer.enabled = true;
					}
				}
			}
		}
	}
}