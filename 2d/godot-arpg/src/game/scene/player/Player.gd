extends CharacterBody2D

@export var ACCELERATION = 500
@export var MAX_SPEED = 80
@export var ROLL_SPEED = 125
@export var FRICTION = 500

const PlayerHurtSound = preload("res://src/game/scene/player/player_hurt_sound.tscn")

enum {
	MOVE,
	ATTACK,
	ROLL
}

var state = MOVE
var roll_vector = Vector2.DOWN
var stats = PlayerStats

@onready var animationPlayer = $AnimationPlayer
@onready var animationTree = $AnimationTree
@onready var animationState = animationTree.get("parameters/playback")
@onready var swordHitbox = $HitBoxPivot/SwordHitBox
@onready var hurtbox = $HurtBox
@onready var blinkAnimationPlayer = $BlinkAnimationPlayer
@onready var hp = $Health

@onready var menu = preload("res://src/game/ui/menu/game_menu.tscn")

func _ready():
	randomize()
	hp.value = stats.health
	stats.no_health.connect(on_player_no_health)
#	stats.connect("no_health",queue_free)
	animationTree.active = true
	swordHitbox.knockback_vector =  roll_vector

func _physics_process(delta):
	match state:
		MOVE:
			move_state(delta)
		ATTACK:
			attack_state(delta)
		ROLL:
			roll_state(delta)

func on_player_no_health():
	# todo 重新开始
	queue_free()

func move_state(delta):
	var input_direction = Vector2(
		Input.get_action_strength("right") - Input.get_action_strength("left"),
		Input.get_action_strength("down") - Input.get_action_strength("up")
	)
	
	input_direction = input_direction.normalized()
	
	if input_direction != Vector2.ZERO:
		roll_vector = input_direction
		swordHitbox.knockback_vector = input_direction
		animationTree.set("parameters/Idle/blend_position", input_direction)
		animationTree.set("parameters/Run/blend_position",input_direction)
		animationTree.set("parameters/Attack/blend_position",input_direction)
		animationTree.set("parameters/Roll/blend_position",input_direction)
		animationState.travel("Run")
		velocity = velocity.move_toward(input_direction * MAX_SPEED, ACCELERATION * delta)
	else:
		animationState.travel("Idle")
		velocity = velocity.move_toward(Vector2.ZERO, FRICTION * delta)

	move_and_slide()
	
	if Input.is_action_just_pressed("roll"):
		state = ROLL
		
	if Input.is_action_just_pressed("attack"):
		state = ATTACK

func roll_state(_delta):
	velocity = roll_vector * ROLL_SPEED
	animationState.travel("Roll")
	move_and_slide()

func attack_state(_delta):
	velocity = Vector2.ZERO
	animationState.travel("Attack")

func roll_animation_finished():
	velocity = velocity * 0.8
	state = MOVE

func attack_state_finished():
	state = MOVE

func _on_hurt_box_area_entered(area):
	stats.health -= area.damage
	hp.value = stats.health
	hurtbox.start_invincibility(0.6)
	hurtbox.create_hit_effect()
	var playerHurtSound = PlayerHurtSound.instantiate()
	get_tree().current_scene.add_child(playerHurtSound)
	
func _on_hurt_box_invincibility_ended():
	blinkAnimationPlayer.play("Stop")

func _on_hurt_box_invincibility_started():
	blinkAnimationPlayer.play("Start")
