# myapp/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (
    UserViewSet, RoleViewSet, UserRolesViewSet,
    EventViewSet, UserDataViewSet
)

router = DefaultRouter()
router.register(r'users', UserViewSet, basename='user')
router.register(r'roles', RoleViewSet)
router.register(r'userroles', UserRolesViewSet)
router.register(r'events', EventViewSet)
router.register(r'userdata', UserDataViewSet)

urlpatterns = [
    path('', include(router.urls)),

]