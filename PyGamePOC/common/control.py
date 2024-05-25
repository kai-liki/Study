import pygame
import platform

from common.resource import get_image_surface, ImageResource, AnimationResource, load_animation


PLATFORM = platform.system()
FONT_NAME='songti'
if PLATFORM is 'Windows':
    FONT_NAME='simhei'
elif PLATFORM is 'Darwin':
    FONT_NAME='songti'



class Controller:
    from common.game import GameController
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

    def __init__(self, name, controllers=None, game_controller=None):
        from common.game import GameController
        self.game = game_controller
        self.name = name
        self.controllers = controllers if controllers is not None else []

    def register_controller(self, controller: Controller):
        self.controllers.append(controller)

    def handle_event(self, event):
        pass


class VisibleControl(Control):
    def __init__(self, name, controllers=None, game_controller=None):
        super().__init__(name, controllers, game_controller)
        self.x, self.y = (0, 0)
        self.width, height = (100, 100)
        self.layer = 0
        self.bg_color = (0, 0, 0)
        self.is_mouse_over = False
        self.img_background: ImageResource = None
        self.is_visible = True

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

    def set_visible(self, is_visible):
        self.is_visible = is_visible

    def get_rect(self):
        return pygame.rect.Rect(self.x, self.y, self.width, self.height)

    def render(self):
        surface = pygame.surface.Surface((self.width, self.height))
        if self.img_background is None:
            surface.fill(self.bg_color)
        else:
            # surface.fill(self.bg_color)
            surface = get_image_surface(self.img_background)
            surface = surface.convert_alpha()
            # bg_rect = surface.get_rect()
            # alpha_surface = bg_surface.convert_alpha()
            # surface.blit(bg_surface, bg_rect)
            # surface.blit(alpha_surface, bg_rect)
        return surface


class Panel(VisibleControl):
    def __init__(self, name: str, visible_controls=None, controllers=None, game_controller=None):
        super().__init__(name, controllers, game_controller)
        self.visible_controls = visible_controls if visible_controls is not None else []

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

    def render(self):
        surface = super().render()
        if len(self.visible_controls) != 0:
            for control in self.visible_controls:
                if control.is_visible:
                    control_surface = control.render().convert_alpha()
                    surface.blit(control_surface, control.get_rect())

        return surface

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
            (x, y) = mouse_pos[0] - self.x, mouse_pos[1] - self.y
            for control in self.visible_controls:
                rect = control.get_rect()
                if rect.collidepoint((x, y)):
                    control.handle_event(event)
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
            (x, y) = mouse_pos[0] - self.x, mouse_pos[1] - self.y
            for control in self.visible_controls:
                rect = control.get_rect()
                if rect.collidepoint((x, y)):
                    control.set_is_mouse_over(True)
                    control.handle_event(event)
                    processed = True
                else:
                    control.set_is_mouse_over(False)
                    control.handle_event(event)
                    processed = True

            if not processed:
                mouse_buttons = pygame.mouse.get_pressed(3)
                mouse_rel = pygame.mouse.get_rel()
                for controller in self.controllers:
                    controller.process(event_name='on_mouse_move', pos=mouse_pos, rel=mouse_rel, buttons=mouse_buttons)


class Scene(Panel):
    def __init__(self, name: str, visible_controls=None, controllers=None, game_controller=None):
        super().__init__(name, visible_controls, controllers, game_controller)


class Animation(VisibleControl):
    def __init__(self, name, resource: AnimationResource, repeat: bool = True, controllers=None, game_controller=None):
        super().__init__(name, controllers, game_controller)
        self.animation_resource = resource
        self.repeat = repeat
        load_animation(self.animation_resource)

    def render(self):
        surface = pygame.surface.Surface((self.width, self.height))
        if self.animation_resource is None:
            surface.fill(self.bg_color)
        else:
            surface = self.animation_resource.render(self.repeat)
            surface = surface.convert_alpha()
        return surface


class Label(VisibleControl):

    def __init__(self, name, controllers=None, game_controller=None):
        super().__init__(name, controllers, game_controller)
        self.labels = ['Text']
        self.label_color = (255, 255, 255)
        self.label_font = pygame.font.SysFont(FONT_NAME, 14)
        self.label_space = 10

    def set_labels(self, labels: list):
        self.labels = labels

    def set_label_space(self, space: int):
        self.label_space = space

    def set_label_color(self, label_color: tuple):
        self.label_color = label_color

    def set_font(self, font: pygame.font.SysFont):
        self.label_font = font

    def render(self):
        surface = pygame.surface.Surface((self.width, self.height))
        border_rect = surface.get_rect()
        i = 0
        for label in self.labels:
            label_surface = self.label_font.render(label, True, self.label_color)
            label_rect = label_surface.get_rect()
            label_pos = (border_rect.width / 2 - label_rect.width / 2, i * self.label_space + border_rect.height / 2 - label_rect.height / 2)
            surface.blit(label_surface, pygame.rect.Rect(label_pos[0], label_pos[1], self.width, self.height))
            i = i + 1
        return surface


class AttributeLabel(VisibleControl):

    def __init__(self, name, binding_object: object, title: str, attribute: str,  controllers=None, game_controller=None):
        super().__init__(name, controllers, game_controller)
        self.binding_object = binding_object
        self.title = title
        self.title_color = (255, 255, 255)
        self.title_font = pygame.font.SysFont(FONT_NAME, 14)
        self.title_width = 20
        self.attribute_color = (255, 255, 255)
        self.attribute_font = pygame.font.SysFont(FONT_NAME, 14)
        self.attribute = attribute

    def set_title_color(self, label_color: tuple):
        self.title_color = label_color

    def set_title_font(self, font: pygame.font.SysFont):
        self.title_font = font

    def set_title_width(self, width: int):
        self.title_width = width

    def set_attribute_color(self, attribute_color: tuple):
        self.attribute_color = attribute_color

    def set_attribute_font(self, font: pygame.font.SysFont):
        self.attribute_font = font

    def render(self):
        surface = super().render()

        title_surface = self.title_font.render(self.title, True, self.title_color)
        title_rect = title_surface.get_rect()
        title_pos = (0, 0)

        try:
            attribute_value = getattr(self.binding_object, self.attribute)
        except ():
            attribute_value = ''
        attribute_surface = self.attribute_font.render(str(attribute_value), True, self.attribute_color)
        attribute_rect = attribute_surface.get_rect()
        attribute_pos = (self.title_width, 0)

        surface.blit(title_surface, pygame.rect.Rect(title_pos, title_rect.size))
        surface.blit(attribute_surface, pygame.rect.Rect(attribute_pos, attribute_rect.size))
        return surface


class Button(VisibleControl):

    def __init__(self, name, controllers=None, game_controller=None):
        super().__init__(name, controllers, game_controller)
        self.label = 'Button'
        self.label_color = (255, 255, 255)
        self.label_font = pygame.font.SysFont(FONT_NAME, 14)

        self.img_file_normal = ImageResource('default_button_normal', 'resources/imgs/Button.gif', (0, 0), (200, 50))
        self.img_file_over = ImageResource('default_button_over', 'resources/imgs/Button.gif', (0, 50), (200, 50))
        self.img_file_press = ImageResource('default_button_press', 'resources/imgs/Button.gif', (0, 100), (200, 50))

        self.is_mouse_pressed = False

        self.is_enabled = True

    def set_label(self, label: str):
        self.label = label

    def set_label_color(self, label_color: tuple):
        self.label_color = label_color

    def set_font(self, font: pygame.font.SysFont):
        self.label_font = font

    def set_button_images(self, image_resources: tuple):
        if image_resources[0] is not None:
            self.img_file_normal = image_resources[0]
        if image_resources[1] is not None:
            self.img_file_over = image_resources[1]
        if image_resources[2] is not None:
            self.img_file_press = image_resources[2]

    def set_is_enabled(self, is_enabled):
        self.is_enabled = is_enabled

    def handle_event(self, event):
        if self.is_enabled:
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
        if self.is_enabled:
            # surface = super().render()
            if self.is_mouse_pressed:
                surface = get_image_surface(self.img_file_press)
            elif self.is_mouse_over:
                surface = get_image_surface(self.img_file_over)
            else:
                surface = get_image_surface(self.img_file_normal)
        else:
            surface = get_image_surface(self.img_file_normal)
        button_rect = surface.get_rect()
        label_surface = self.label_font.render(self.label, True, self.label_color)
        label_rect = label_surface.get_rect()
        label_pos = (button_rect.width / 2 - label_rect.width / 2, button_rect.height / 2 - label_rect.height / 2)
        surface.blit(label_surface, pygame.rect.Rect(label_pos[0], label_pos[1], self.width, self.height))
        return surface
