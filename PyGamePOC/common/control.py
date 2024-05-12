import pygame

from PyGamePOC.common.game import GameController
from PyGamePOC.common.resource import get_full_image_surface, get_image_surface, ImageResource


class Controller:
    game: GameController

    def set_game(self, game: GameController):
        self.game = game

    def get_game(self):
        return self.game

    def process(self, **kwargs):
        pass

    def get_scene(self):
        pass


class Control:
    name: str
    game: GameController
    controllers: list

    def __init__(self, name):
        self.name = name
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
    img_background: ImageResource = None

    def set_position(self, x: int, y: int):
        self.x, self.y = (x, y)

    def get_position(self):
        return self.x, self.y

    def set_background_image(self, resource: ImageResource):
        self.img_background = resource

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
        if self.img_background is None:
            surface.fill(self.bg_color)
        else:
            bg_surface = get_image_surface(self.img_background)
            bg_rect = bg_surface.get_rect()
            surface.blit(bg_surface, bg_rect)
        return surface


class Scene(VisibleControl):
    visible_controls: list

    def __init__(self, name: str):
        super().__init__(name)
        self.visible_controls = []

    def add_control(self, control: VisibleControl):
        templist = [item for item in self.visible_controls if item.name == control.name]
        assert len(templist) == 0
        self.visible_controls.append(control)
        self.visible_controls.sort(key=lambda x: x.layer, reverse=False)

    def add_controls(self, controls: list):
        names = [item.name for item in controls]
        templist = [item for item in self.visible_controls if item.name in names]
        assert len(templist) == 0
        for control in controls:
            self.visible_controls.append(control)
        self.visible_controls.sort(key=lambda x: x.layer, reverse=False)

    def remove_control(self, control: VisibleControl):
        for i, o in enumerate(self.visible_controls):
            if o.name == control.name:
                del self.visible_controls[i]
                break

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

    img_file_normal = ImageResource('default_button_normal', 'resources/imgs/Button.gif', (0, 0), (200, 50))
    img_file_over = ImageResource('default_button_over', 'resources/imgs/Button.gif', (0, 51), (200, 50))
    img_file_press = ImageResource('default_button_press', 'resources/imgs/Button.gif', (0, 101), (200, 50))

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
        # surface = super().render()
        if self.is_mouse_pressed:
            surface = get_image_surface(self.img_file_press)
        elif self.is_mouse_over:
            surface = get_image_surface(self.img_file_over)
        else:
            surface = get_image_surface(self.img_file_normal)
        button_rect = surface.get_rect()
        label_surface = self.label_font.render(self.label, True, self.label_color)
        label_rect = label_surface.get_rect()
        label_pos = (button_rect.width / 2 - label_rect.width / 2, button_rect.height / 2 - label_rect.height / 2)
        surface.blit(label_surface, pygame.rect.Rect(label_pos[0], label_pos[1], self.width, self.height))
        return surface
