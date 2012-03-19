public override float TakeDamage (Attack atk)
{
	float dmg = atk.damageValue;
	
	// This guy is weak against ice
	if (atk.element == ElementType.ICE){
		dmg *= 2;
	}
	
	stats.DoDamage(dmg, false);
	
	return dmg;
}