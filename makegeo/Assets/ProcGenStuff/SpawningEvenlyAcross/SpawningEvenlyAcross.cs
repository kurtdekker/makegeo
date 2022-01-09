using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEvenlyAcross : MonoBehaviour
{
	[Header("We spread the items across this.")]
	public RectTransform Left;
	public RectTransform Right;

	[Header( "What we will clone.")]
	public GameObject ExampleMarker;

	List<GameObject> Items = new List<GameObject>();

	void Start()
	{
		ExampleMarker.SetActive(false);

		DSM.SpawningEvenlyAcross.ItemCount.iValue = 5;
	}

	// what we made so we're not constantly churning UI
	int weMadeItemCount;
	SpacingStrategies.Distribution weMadeDistribution;

	void UpdateItems()
	{
		int ItemCount = DSM.SpawningEvenlyAcross.ItemCount.iValue;
		SpacingStrategies.Distribution Distribution = (SpacingStrategies.Distribution)DSM.SpawningEvenlyAcross.DistributionStrategy.iValue;

		// keep everything sane, wrap around
		if (ItemCount < 3) ItemCount = 3;
		if (ItemCount > 8) ItemCount = 3;
		if (Distribution < 0) Distribution = 0;
		if (Distribution >= SpacingStrategies.Distribution.MAXIMUM) Distribution = 0;

		DSM.SpawningEvenlyAcross.ItemCount.iValueIfDifferent = ItemCount;
		DSM.SpawningEvenlyAcross.DistributionStrategy.iValueIfDifferent = (int)Distribution;

		bool rebuild = false;

		if (ItemCount != weMadeItemCount)
		{
			rebuild = true;
		}
		if (Distribution != weMadeDistribution)
		{
			rebuild = true;
		}

		if (rebuild)
		{
			weMadeItemCount = ItemCount;
			weMadeDistribution = Distribution;

			DSM.SpawningEvenlyAcross.DistributionStrategyText.Value = Distribution.ToString();

			// blow away the old ones
			foreach( var item in Items)
			{
				Destroy(item);
			}

			// empty the list
			Items.Clear();

			// create new ones and store them
			for (int i = 0; i < ItemCount; i++)
			{
				var fraction = SpacingStrategies.Distribute( Distribution, 0.0f, 1.0f, i, ItemCount);

				Vector3 pos = Vector3.Lerp( Left.transform.position, Right.transform.position, fraction);

				var item = Instantiate<GameObject>( ExampleMarker, ExampleMarker.transform.parent);
				item.SetActive(true);
				item.transform.position = pos;

				Items.Add( item);
			}
		}
	}

	void Update()
	{
		UpdateItems();
	}
}
