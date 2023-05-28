extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

func save():
	get_viewport().set_clear_mode(Viewport.CLEAR_MODE_ONLY_NEXT_FRAME)
	# Wait until the frame has finished before getting the texture.
	yield(VisualServer, "frame_post_draw")

	# Retrieve the captured image.
	var img = get_viewport().get_texture().get_data()

	# Flip it on the y-axis (because it's flipped).
	img.flip_y()
	img.save_png("test.png")


# Called when the node enters the scene tree for the first time.
func _ready():
	var rect = rect_size
	print(rect)
	var size = Vector2(rect.x, rect.y)
	print(size)
	OS.window_size = size
	print(OS.window_size)
	call_deferred("save")


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
