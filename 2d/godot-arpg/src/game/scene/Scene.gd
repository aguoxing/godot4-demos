extends Node2D

const Bat = preload("res://src/game/scene/enemies/bat.tscn")

@onready var player = $Player
@onready var spawn_timer = $Timer

var stats = PlayerStats

# Called when the node enters the scene tree for the first time.
func _ready():
	spawn_timer.start()
	
func _on_timer_timeout():
	if stats.bat_num < stats.max_bat_num:
		spawn()

func spawn():
	if is_instance_valid(player):
		var enemy = Bat.instantiate()
		add_child(enemy)
		enemy.position = player.position + Vector2(100, 0).rotated(randf_range(0, 2 * PI))
		
		stats.bat_num += 1
	else:
		spawn_timer.stop()

