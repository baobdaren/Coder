using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearAccessor : AbsPartAccessorBase
{
	// ----------------//
	// --- 序列化
	// ----------------//
	[SerializeField]
	private GameObject _originGearHead;
	[SerializeField]
	private CircleCollider2D _gearTopRadius;
	[SerializeField]
	private CircleCollider2D _gearBodyCircle;
	[SerializeField]
	private CircleCollider2D _gearMiddleCircle;
	[SerializeField]
	private List<Sprite> _spritesList;
	// ----------------//
	// --- 公有成员
	// ----------------//
	public GameObject GearBody;
	public float ToothRadius = 0.0392f;
	[HideInInspector]
	public List<GameObject> OriginGeadHeads = new List<GameObject>();
	public CircleCollider2D GearBodyCircle => _gearBodyCircle;
	public CircleCollider2D MiddleCircle => _gearMiddleCircle;
	public CircleCollider2D GearTopRadius => _gearTopRadius;

	// ----------------//
	// --- 私有成员
	// ----------------//
	private List<Collider2D> _colliderGearMiddle;
	private List<Collider2D> _collidersGearHeadAndBody;

	// ----------------//
	// --- Unity消息
	// ----------------//
#if UNITY_EDITOR

	private void Awake()
	{
		Debug.Assert(_gearBodyCircle.isTrigger == false);
		Debug.Assert(_gearMiddleCircle.isTrigger == true);
		Debug.Assert(_gearTopRadius.isTrigger == true);
	}
#endif
	// ----------------//
	// --- 公有方法
	// ----------------//
	public void SetGearDisplay(int gearSizeIndex)
	{ 
		GetComponentInChildren<SpriteRenderer>().sprite = _spritesList[gearSizeIndex];
	}

	// ----------------//
	// --- 私有方法
	// ----------------//
	protected override List<Collider2D> GetCollidersForConflict(PartTypes targetPartType)
	{
		if (targetPartType == PartTypes.Gear || targetPartType == PartTypes.Rail)
		{
			if (_colliderGearMiddle == null)
			{
				_colliderGearMiddle = new List<Collider2D>() { MiddleCircle };
			}
			return _colliderGearMiddle;
		}
		else
		{
			if (_collidersGearHeadAndBody == null)
			{
				_collidersGearHeadAndBody = new List<Collider2D>(AllColliders);
				_collidersGearHeadAndBody.Remove(MiddleCircle);
			}
			return _collidersGearHeadAndBody;
		}
	}

	protected override void OnBeforeInitAccssor()
	{
		OriginGeadHeads.Add(_originGearHead);
		while (OriginGeadHeads.Count < 24)
		{
			OriginGeadHeads.Add(GameObject.Instantiate(_originGearHead, _originGearHead.transform.parent));
		}
	}

	// ----------------//
	// --- 类型
	// ----------------//

}
