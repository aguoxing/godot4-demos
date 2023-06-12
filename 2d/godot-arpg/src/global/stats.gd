extends Node

@export var max_health: int = 100: set = set_max_health
var health = max_health: set = set_health

@export var max_bat_num: int = 20:
	get:
		return max_bat_num
	set(value):
		max_bat_num = value
		self.bat_num = min(bat_num,max_bat_num)
		emit_signal("max_bat_num_changed", max_bat_num)
var bat_num = 0:
	set(value):
		bat_num = value
		emit_signal("bat_num_changed",bat_num)
		
var score = 0:
	set(value):
		score = value
		emit_signal("score_changed", score)

signal no_health
signal health_changed(value)
signal max_health_changed(value)

signal bat_num_changed(value)
signal max_bat_num_changed(value)

signal score_changed(value)

func set_max_health(value):
	max_health = value
	self.health = min(health, max_health)
	emit_signal("max_health_changed", max_health)

func set_health(value):
	health = value
	emit_signal("health_changed", health)
	if health <= 0:
		emit_signal("no_health")

func _ready():
	self.health = max_health
