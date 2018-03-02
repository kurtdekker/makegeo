
# 6:51 AM 3/2/2018 - I want a beach ball texture!

import pygame

class glb: pass
g = glb()

g.namecounter = 0

g.percent = 92

g.borderthick = 2

sz = 512

def	makebeachball( polarcolor, bordercolor, borderthick, piecolors, percent):
	g.namecounter += 1
	filename = "beachball%u.png" % g.namecounter

	sfc = pygame.Surface( (sz,sz))

	sfc.fill( polarcolor)

	y = (float(512) * (100 - percent)) / 100
	y = int(y)

	w = sz
	h = sz - y * 2

	for i in xrange( len( piecolors)):
		color = piecolors[i]
		x = (i * 512) / len( piecolors)
		pygame.draw.rect( sfc, color, (x, y, w, h))

	pygame.draw.rect( sfc, bordercolor, (0, y, w, borderthick))
	pygame.draw.rect( sfc, bordercolor, (0, sz - (y + borderthick + 1), w, borderthick))

	pygame.image.save( sfc, filename)

	return

def	main():
	WHITE = (255,255,255)
	BLACK = (0,0,0)
	GRAY = (200,200,200)
	RED = (255,0,0)
	YELLOW = (255,255,0)
	BLUE = (40,40,255)
	GREEN = (0,255,0)

	makebeachball( WHITE, BLACK, g.borderthick,
		(WHITE, BLUE, WHITE, RED, WHITE, YELLOW),
		g.percent)

	makebeachball( WHITE, BLACK, g.borderthick,
		(WHITE, GREEN, WHITE, BLUE, WHITE, RED, WHITE, YELLOW),
		g.percent)

	return

if __name__ == "__main__":
	main()
