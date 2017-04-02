function SwordProjectile::onCollision(%data, %projectile, %object, %fade, %position, %normal, %velocity)
{
	Parent::onCollision(%data, %projectile, %object, %fade, %position, %normal, %velocity);
	if(%object.getType() & $TypeMasks::FxBrickObjectType && %object.getDataBlock().isDoor)
		ServerPlay3D(WoodHitSound, %position);
	if (!(%object.getType() & $TypeMasks::FxBrickObjectType))
		return;
	%object.doorDamage(1);
}