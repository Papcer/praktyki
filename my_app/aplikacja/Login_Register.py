import threading

from django.contrib.auth import login, logout
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from drf_yasg.utils import swagger_auto_schema
from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import AllowAny
from rest_framework_simplejwt.tokens import RefreshToken
from .models import User, Role, UserRoles, Event,UserData
from drf_yasg import openapi
import pika
from django.core.mail import send_mail
from django.conf import settings
from aplikacja.tasks import send_verification_email

@swagger_auto_schema(
    method='post',
    request_body=openapi.Schema(
        type=openapi.TYPE_OBJECT,
        properties={
            'username': openapi.Schema(type=openapi.TYPE_STRING, example='testusername'),
            'password': openapi.Schema(type=openapi.TYPE_STRING,example='testpassword'),
        },
        required=['username', 'password'],
    ),
    responses={
        200: openapi.Response('Pomyślnie zarejestrowano użytkownika'),
        400: openapi.Response('Bad Request'),
        500: openapi.Response('Internal Server Error'),
    },
)
@api_view(['POST'])
@permission_classes([AllowAny]) #dostepne dla kazdego
@csrf_exempt
def register(request):
    """
     Api pozwalające zarejestrować nowego użytkownika, po rejestracji na podany adres zostaje wysłany link do weryfikacji konta
     Wymagane paramtry:
         -username: login użytkownika
         -password: hasło użytkownika
     """
    if request.method == 'POST':
        username = request.data.get('username')
        password = request.data.get('password')

        if not username or not password:
            return JsonResponse({'error': 'Wymagane sa nazwa i haslo.'}, status=400)

        user = User.create_user(username=username, password=password)

        send_verification_email.apply_async(args=[user.id], countdown=5)

    return JsonResponse({'message': 'Pomyslnie zarejestrowano uzytkownika.'}, status=201)



@api_view(['PUT'])
@csrf_exempt
@permission_classes([AllowAny])
def verify_email(request, user_id):
    """
    Api pozwalające zweryfikować adres email użytkownika. Wywoływane po pomyślnej rejestraji użytkownika
    Wymagane paramtry:
        -user_id: identyfikator użytkownika
    """
    try:
        user = User.objects.get(pk=user_id)
        user.email_verified = True
        user.save()
        
        return JsonResponse({'message': "Adres e-mail zostal pomyslnie zweryfikowany"})
    except Exception as e:
        return JsonResponse({'error': str(e)})


@swagger_auto_schema(
    method='post',
    request_body=openapi.Schema(
        type=openapi.TYPE_OBJECT,
        properties={
            'username': openapi.Schema(type=openapi.TYPE_STRING, example='testusername'),
            'password': openapi.Schema(type=openapi.TYPE_STRING,example='testpassword'),
        },
        required=['username', 'password'],
    ),
    responses={
        200: openapi.Response('Pomyślnie zalogowano'),
        400: openapi.Response('Bad Request'),
        500: openapi.Response('Internal Server Error'),
    },
)
@api_view(['POST'])
@permission_classes([AllowAny]) #dostepne dla kazdego
@csrf_exempt
def login_view(request):
    """
    Api pozwalające się zalogować i otrzymać token JWT
    Wymagane paramtry:
        -username: login użytkownika
        -password: hasło użytkownika
    """
    if request.method == 'POST':
        username = request.data.get('username')
        password = request.data.get('password')

        if not username or not password:
            return JsonResponse({'error': 'Email i haslo wymagane.'}, status=400)

        user = custom_authenticate(username, password)

        if user:
            # Logowanie użytkownika
            if user.check_password(password):
                login(request, user)
    
                #wszystkie role powiazanie z danym uzytkownikiem
                user_roles = UserRoles.objects.filter(user=user)
    
                roles_list = list(user_roles.values_list('role__role_name', flat=True))
                # Generowanie tokenów JWT
                refresh = RefreshToken.for_user(user)
                refresh['roles'] = roles_list
    
                return JsonResponse({
                    'message': 'Poprawnie zalogowano.',
                    'access_token': str(refresh.access_token),
                    'refresh_token': str(refresh),
                }, status=200)
            else:
                return JsonResponse({'error': 'Bledne haslo.'}, status=401)
        else:
            return JsonResponse({'error': 'Bledne dane.'}, status=401)

@api_view(['POST'])
def logout_view(request):
    if request.method == 'POST':
        #usuniecie ciasteczek
        request.session.flush()
        # Wylogowanie użytkownika
        logout(request)

        return JsonResponse({'message': 'Poprawnie wylogowano.'}, status=200)
    else:
        return JsonResponse({'error': 'Bledna metoda.'}, status=405)

def custom_authenticate(username, password):
    try:
        user = User.objects.get(username=username)
        if user.check_password(password):
            return user
    except User.DoesNotExist:
        return None