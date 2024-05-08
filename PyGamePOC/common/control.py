import pygame

from PyGamePOC.common.game import GameController
from PyGamePOC.common.resource import get_image_surface


class Controller:
    game: GameController

    def set_game(self, game: GameController):
        self.game = game

    def get_game(self):
        return self.game

    def process(self, **kwargs):
        pass


class Control:
    game: GameController
    controllers: list

    def __init__(self):
        self.controllers = []

    def register_controller(self, controller: Controller):
        self.controllers.append(controller)

    def handle_event(self, event):
        pass


class VisibleControl(Control):
    x, y = (0, 0)
    width, height = (100, 100)
    layer = 0
    bg_color = (0, 0, 0)
    is_mouse_over = False

    def set_position(self, x: int, y: int):
        self.x, self.y = (x, y)

    def get_position(self):
        return self.x, self.y

    def get_is_mouse_over(self):
        return self.is_mouse_over

    def set_is_mouse_over(self, is_mouse_over):
        self.is_mouse_over = is_mouse_over

    def set_layer(self, layer: int):
        self.layer = layer

    def set_size(self, width: int, height):
        self.width, self.height = (width, height)

    def set_bg_color(self, bg_color: tuple):
        self.bg_color = bg_color

    def get_rect(self):
        return pygame.rect.Rect(self.x, self.y, self.width, self.height)

    def render(self):
        surface = pygame.surface.Surface((self.width, self.height))
        surface.fill(self.bg_color)
        return surface


class Scene(VisibleControl):
    visible_controls: list

    def __init__(self):
        super().__init__()
        self.visible_controls = []

    def add_control(self, control: VisibleControl):
        self.visible_controls.append(control)
        self.visible_controls.sort(key=lambda x: x.layer, reverse=False)

    def handle_event(self, event):
        if event.type == pygame.KEYUP:
            for controller in self.controllers:
                controller.process(event_name='on_key_up', key=event.key)
        elif event.type == pygame.KEYDOWN:
            for controller in self.controllers:
                controller.process(event_name='on_key_down', key=event.key)
        elif event.type in [pygame.MOUSEBUTTONUP, pygame.MOUSEBUTTONDOWN]:
            processed = False
            mouse_pos = pygame.mouse.get_pos()
            for control in self.visible_controls:
                rect = control.get_rect()
                if rect.collidepoint(mouse_pos):
                    control.handle_event(pygame.QUIT)
                    processed = True
            if not processed:
                mouse_buttons = pygame.mouse.get_pressed(3)
                if event.type == pygame.MOUSEBUTTONUP:
                    for controller in self.controllers:
                        controller.process(event_name='on_mouse_button_up', pos=mouse_pos, buttons=mouse_buttons)
                elif event.type == pygame.MOUSEBUTTONDOWN:
                    for controller in self.controllers:
                        controller.process(event_name='on_mouse_button_down', pos=mouse_pos, buttons=mouse_buttons)
        elif event.type == pygame.MOUSEMOTION:
            processed = False
            mouse_pos = pygame.mouse.get_pos()
            for control in self.visible_controls:
                rect = control.get_rect()
                if rect.collidepoint(mouse_pos):
                    if not control.get_is_mouse_over():
                        control.set_is_mouse_over(True)
                        control.handle_event(pygame.QUIT)
                    processed = True
                else:
                    if control.get_is_mouse_over():
                        control.set_is_mouse_over(False)
                        control.handle_event(pygame.QUIT)
                        processed = True

            if not processed:
                mouse_buttons = pygame.mouse.get_pressed(3)
                mouse_rel = pygame.mouse.get_rel()
                for controller in self.controllers:
                    controller.process(event_name='on_mouse_move', pos=mouse_pos, rel=mouse_rel, buttons=mouse_buttons)

    def render(self):
        surface = super().render()
        if len(self.visible_controls) != 0:
            for control in self.visible_controls:
                control_surface = control.render()
                surface.blit(control_surface, control.get_rect())

        return surface


class Button(VisibleControl):
    label = 'Text'
    label_color = (255, 255, 255)
    label_font = pygame.font.SysFont('arial', 14)

    img_file_normal = 'resources/imgs/Button_normal.gif'
    img_file_over = 'resources/imgs/Button_over.gif'
    img_file_press = 'resources/imgs/Button_press.gif'

    is_mouse_pressed = False

    def set_label(self, label: str):
        self.label = label

    def set_label_color(self, label_color: tuple):
        self.label_color = label_color

    def handle_event(self, event):
        event_name: str = None
        if not self.is_mouse_over:
            self.is_mouse_pressed = False
            event_name = 'mouse_leave'
        else:
            event_name = 'mouse_over'
            mouse_buttons = pygame.mouse.get_pressed(3)
            pygame.mouse.get_pressed()
            if mouse_buttons[0]:
                self.is_mouse_pressed = True
                event_name = 'mouse_press'
            else:
                if self.is_mouse_pressed is True:
                    self.is_mouse_pressed = False
                    event_name = 'mouse_click'

        if event_name is not None:
            mouse_pos = pygame.mouse.get_pos()
            for controller in self.controllers:
                controller.process(event_name=event_name, position=mouse_pos)

    def render(self):
        surface = None
        if self.is_mouse_pressed:
            surface = get_image_surface(self.img_file_press)
        elif self.is_mouse_over:
            surface = get_image_surface(self.img_file_over)
        else:
            surface = get_image_surface(self.img_file_normal)
        surface.blit(self.label_font.render(self.label, True, self.label_color), pygame.rect.Rect(30, 10, self.width, self.height))
        return surface
