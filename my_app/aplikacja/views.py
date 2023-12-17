from django.contrib.auth.models import User
from rest_framework import viewsets
from .models import User, Role, UserRoles, Event, UserData
from .serializers import ( UserSerializer, RoleSerializer, UserRolesSerializer, EventSerializer, UserDataSerializer,
                           
)

class UserViewSet(viewsets.ModelViewSet):
    serializer_class = UserSerializer

    def get_queryset(self):
        # Sprawdź, czy podano parametr ID w zapytaniu
        user_id = self.kwargs.get('pk')

        if user_id:
            return User.objects.filter(pk=user_id)
        else:
            return User.objects.all()
    
class RoleViewSet(viewsets.ModelViewSet):
    queryset = Role.objects.all()
    serializer_class = RoleSerializer

class UserRolesViewSet(viewsets.ModelViewSet):
    queryset = UserRoles.objects.all()
    serializer_class = UserRolesSerializer

class EventViewSet(viewsets.ModelViewSet):
    queryset = Event.objects.all()
    serializer_class = EventSerializer

class UserDataViewSet(viewsets.ModelViewSet):
    queryset = UserData.objects.all()
    serializer_class = UserDataSerializer

