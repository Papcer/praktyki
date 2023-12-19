from rest_framework import serializers
from .models import User, Role, UserRoles, Event, UserData

class UserSerializer(serializers.ModelSerializer):
    class Meta:
        model=User
        fields = ['id', 'username', 'password']
    
class RoleSerializer(serializers.ModelSerializer):
    class Meta:
        model=Role
        fields = ['id', 'role_name']

class UserRolesSerializer(serializers.ModelSerializer):
    class Meta:
        model=UserRoles
        fields = ['id', 'user', 'role']

class EventSerializer(serializers.ModelSerializer):
    class Meta:
        model=Event
        fields = ['id', 'title', 'start_date', 'location', 'ticket_limit']


class UserDataSerializer(serializers.ModelSerializer):
    class Meta:
        model = UserData
        fields = ['id', 'firstName', 'lastName', 'phoneNumber']
